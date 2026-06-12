using Apps.Traduno.Models.Api;
using Apps.Traduno.Models.Requests;
using Blackbird.Applications.Sdk.Common.Exceptions;

namespace Apps.Traduno.Utils;

public static class TradunoRequestMapper
{
    public static Dictionary<string, string?> BuildProjectQuery(SearchProjectsInput input) => new()
    {
        { "filter[by_status]", input.Status },
        { "filter[by_service]", input.ServiceCode },
        { "filter[by_translation_area]", input.TranslationAreaId },
        { "filter[by_po_number]", input.PoNumber },
        { "filter[created_start]", FormatDate(input.CreatedStart) },
        { "filter[created_end]", FormatDate(input.CreatedEnd) },
        { "filter[deadline_start]", FormatDate(input.DeadlineStart) },
        { "filter[deadline_end]", FormatDate(input.DeadlineEnd) },
        { "filter[show_all]", input.ShowAll?.ToString().ToLowerInvariant() }
    };

    public static Dictionary<string, string?> BuildQuoteQuery(SearchQuotesInput input) => new()
    {
        { "filter[by_status]", input.Status },
        { "filter[by_service]", input.ServiceCode },
        { "filter[by_translation_area]", input.TranslationAreaId },
        { "filter[by_po_number]", input.PoNumber },
        { "filter[created_start]", FormatDate(input.CreatedStart) },
        { "filter[created_end]", FormatDate(input.CreatedEnd) },
        { "filter[project_deadline_start]", FormatDate(input.ProjectDeadlineStart) },
        { "filter[project_deadline_end]", FormatDate(input.ProjectDeadlineEnd) },
        { "filter[show_all]", input.ShowAll?.ToString().ToLowerInvariant() }
    };

    public static Dictionary<string, string?> BuildInvoiceQuery(SearchInvoicesInput input) => new()
    {
        { "filter[by_name]", input.Name },
        { "filter[by_status]", input.Status },
        { "filter[issued_start]", FormatDate(input.IssuedStart) },
        { "filter[issued_end]", FormatDate(input.IssuedEnd) },
        { "filter[deadline_start]", FormatDate(input.DeadlineStart) },
        { "filter[deadline_end]", FormatDate(input.DeadlineEnd) }
    };

    public static CreateProjectRequest BuildProjectRequest(CreateProjectInput input, IEnumerable<string> sourceFileIds)
    {
        var sourceFiles = sourceFileIds.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
        if (!sourceFiles.Any())
        {
            throw new PluginApplicationException("At least one source file is required to create a project.");
        }

        return new CreateProjectRequest
        {
            Name = input.Name,
            PoNumber = input.PoNumber,
            Notes = input.Notes,
            JobInstructions = input.JobInstructions,
            TranslationAreaId = ParseNullableInt(input.TranslationAreaId, "translation area"),
            CurrencyId = ParseNullableInt(input.CurrencyId, "currency"),
            BillingEntityId = input.BillingEntityId,
            CallbackUrl = input.CallbackUrl,
            DeliveryFilesFormat = input.DeliveryFilesFormat,
            SourceFiles = sourceFiles,
            Deliverables = BuildDeliverables(input)
        };
    }

    public static CreateQuoteRequest BuildQuoteRequest(CreateQuoteInput input, IEnumerable<string>? sourceFileIds) => new()
    {
        Name = input.Name,
        PoNumber = input.PoNumber,
        Notes = input.Notes,
        JobInstructions = input.JobInstructions,
        TranslationAreaId = ParseNullableInt(input.TranslationAreaId, "translation area"),
        CurrencyId = ParseNullableInt(input.CurrencyId, "currency"),
        BillingEntityId = input.BillingEntityId,
        CallbackUrl = input.CallbackUrl,
        DeliveryFilesFormat = input.DeliveryFilesFormat,
        SourceFiles = sourceFileIds?.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray(),
        Deliverables = BuildDeliverables(input)
    };

    private static IEnumerable<CreateDeliverableRequest> BuildDeliverables(CreateTradunoEntityInputBase input)
    {
        var serviceCodeGroups = input.DeliverableServiceCodeGroups
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToList();
        var schedulingModes = input.DeliverableSchedulingModes
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToList();
        var schedulingValues = input.DeliverableSchedulingValues
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToList();

        if (!serviceCodeGroups.Any())
        {
            throw new PluginApplicationException("At least one deliverable service code group is required.");
        }

        var deliverableCount = serviceCodeGroups.Count;
        EnsureCount(nameof(input.DeliverableSchedulingModes), schedulingModes.Count, deliverableCount);
        EnsureCount(nameof(input.DeliverableSchedulingValues), schedulingValues.Count, deliverableCount);

        var sourceLanguageCodes = NormalizeOptionalList(input.DeliverableSourceLanguageCodes, deliverableCount);
        var targetLanguageGroups = NormalizeOptionalList(input.DeliverableTargetLanguageCodeGroups, deliverableCount);
        var descriptions = NormalizeOptionalList(input.DeliverableDescriptions, deliverableCount);

        var deliverables = new List<CreateDeliverableRequest>();
        for (var index = 0; index < deliverableCount; index++)
        {
            var serviceCodes = SplitCommaSeparated(serviceCodeGroups[index]);
            if (!serviceCodes.Any())
            {
                throw new PluginApplicationException($"Deliverable {index + 1} must contain at least one service code.");
            }

            var deliverable = new CreateDeliverableRequest
            {
                ServiceCodes = serviceCodes,
                SourceLanguageCode = sourceLanguageCodes[index],
                TargetLanguageCodes = SplitCommaSeparated(targetLanguageGroups[index]),
                Description = descriptions[index]
            };

            var mode = schedulingModes[index].Trim().ToLowerInvariant();
            var value = schedulingValues[index].Trim();

            if (mode == "deadline")
            {
                if (!DateTime.TryParse(value, out var deadline))
                {
                    throw new PluginApplicationException(
                        $"Deliverable {index + 1} scheduling value must be a valid date-time when mode is deadline.");
                }

                deliverable.Deadline = deadline;
            }
            else if (mode == "turnaround")
            {
                if (!int.TryParse(value, out var turnaroundTime))
                {
                    throw new PluginApplicationException(
                        $"Deliverable {index + 1} scheduling value must be an integer when mode is turnaround.");
                }

                deliverable.TurnaroundTime = turnaroundTime;
            }
            else
            {
                throw new PluginApplicationException(
                    $"Deliverable {index + 1} scheduling mode must be either deadline or turnaround.");
            }

            deliverables.Add(deliverable);
        }

        return deliverables;
    }

    private static List<string?> NormalizeOptionalList(IEnumerable<string>? values, int expectedCount)
    {
        if (values == null)
        {
            return Enumerable.Repeat<string?>(null, expectedCount).ToList();
        }

        var list = values.ToList();
        if (!list.Any())
        {
            return Enumerable.Repeat<string?>(null, expectedCount).ToList();
        }

        EnsureCount("optional deliverable list", list.Count, expectedCount);
        return list.Select(x => string.IsNullOrWhiteSpace(x) ? null : x.Trim()).ToList();
    }

    private static IEnumerable<string> SplitCommaSeparated(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return [];
        }

        return value.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
    }

    private static int? ParseNullableInt(string? value, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        if (!int.TryParse(value, out var parsed))
        {
            throw new PluginApplicationException($"{fieldName} must be a valid integer value.");
        }

        return parsed;
    }

    private static string? FormatDate(DateTime? value) => value?.ToString("yyyy-MM-dd");

    private static void EnsureCount(string fieldName, int actualCount, int expectedCount)
    {
        if (actualCount != expectedCount)
        {
            throw new PluginApplicationException(
                $"{fieldName} must contain exactly {expectedCount} item(s) to match the deliverable count.");
        }
    }
}
