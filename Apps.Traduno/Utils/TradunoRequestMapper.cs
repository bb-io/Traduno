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
            TranslationAreaId = ParseRequiredInt(input.TranslationAreaId, "translation area"),
            CurrencyId = ParseRequiredInt(input.CurrencyId, "currency"),
            BillingEntityId = input.BillingEntityId,
            DeliveryFilesFormat = ParseRequiredString(input.DeliveryFilesFormat, "delivery files format"),
            SourceFiles = sourceFiles,
            Deliverables = BuildDeliverables(input, requireSourceLanguage: false, requireTargetLanguages: true)
        };
    }

    public static CreateQuoteRequest BuildQuoteRequest(CreateQuoteInput input, IEnumerable<string>? sourceFileIds) => new()
    {
        Name = input.Name,
        PoNumber = input.PoNumber,
        Notes = input.Notes,
        JobInstructions = input.JobInstructions,
        TranslationAreaId = ParseRequiredInt(input.TranslationAreaId, "translation area"),
        CurrencyId = ParseRequiredInt(input.CurrencyId, "currency"),
        BillingEntityId = input.BillingEntityId,
        DeliveryFilesFormat = ParseRequiredString(input.DeliveryFilesFormat, "delivery files format"),
        SourceFiles = sourceFileIds?.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray(),
        Deliverables = BuildDeliverables(input, requireSourceLanguage: true, requireTargetLanguages: true)
    };

    private static IEnumerable<CreateDeliverableRequest> BuildDeliverables(
        ITradunoDeliverableInput input,
        bool requireSourceLanguage,
        bool requireTargetLanguages)
    {
        var serviceCodeGroups = input.DeliverableServiceCodeGroups
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToList();
        var schedulingModes = input.DeliverableSchedulingModes
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToList();

        if (!serviceCodeGroups.Any())
        {
            throw new PluginApplicationException("At least one deliverable service code group is required.");
        }

        var deliverableCount = serviceCodeGroups.Count;
        EnsureCount(nameof(input.DeliverableSchedulingModes), schedulingModes.Count, deliverableCount);

        var sourceLanguageCodes = NormalizeOptionalList(input.DeliverableSourceLanguageCodes, deliverableCount);
        var targetLanguageGroups = NormalizeOptionalList(input.DeliverableTargetLanguageCodeGroups, deliverableCount);
        var descriptions = NormalizeOptionalList(input.DeliverableDescriptions, deliverableCount);
        var deadlines = NormalizeOptionalValueList(input.DeliverableDeadlines, deliverableCount);
        var turnaroundTimes = NormalizeOptionalValueList(input.DeliverableTurnaroundTimes, deliverableCount);

        var deliverables = new List<CreateDeliverableRequest>();
        for (var index = 0; index < deliverableCount; index++)
        {
            var serviceCodes = SplitCommaSeparated(serviceCodeGroups[index]);
            if (!serviceCodes.Any())
            {
                throw new PluginApplicationException($"Deliverable {index + 1} must contain at least one service code.");
            }

            var sourceLanguageCode = sourceLanguageCodes[index];
            var targetLanguageCodes = SplitCommaSeparated(targetLanguageGroups[index]).ToArray();

            if (requireSourceLanguage && string.IsNullOrWhiteSpace(sourceLanguageCode))
            {
                throw new PluginApplicationException($"Deliverable {index + 1} source language must be provided.");
            }

            if (requireTargetLanguages && !targetLanguageCodes.Any())
            {
                throw new PluginApplicationException($"Deliverable {index + 1} target language must be provided.");
            }

            var deliverable = new CreateDeliverableRequest
            {
                ServiceCodes = serviceCodes,
                SourceLanguageCode = sourceLanguageCode,
                TargetLanguageCodes = targetLanguageCodes,
                Description = descriptions[index]
            };

            var mode = schedulingModes[index].Trim().ToLowerInvariant();

            if (mode == "deadline")
            {
                var deadline = deadlines[index];
                if (!deadline.HasValue)
                {
                    throw new PluginApplicationException(
                        $"Deliverable {index + 1} deadline must be provided when scheduling mode is deadline.");
                }

                deliverable.Deadline = deadline.Value;
            }
            else if (mode == "turnaround")
            {
                var turnaroundTime = turnaroundTimes[index];
                if (!turnaroundTime.HasValue)
                {
                    throw new PluginApplicationException(
                        $"Deliverable {index + 1} turnaround time must be provided when scheduling mode is turnaround.");
                }

                deliverable.TurnaroundTime = turnaroundTime.Value;
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

    private static List<T?> NormalizeOptionalValueList<T>(IEnumerable<T?>? values, int expectedCount)
        where T : struct
    {
        if (values == null)
        {
            return Enumerable.Repeat<T?>(null, expectedCount).ToList();
        }

        var list = values.ToList();
        if (!list.Any())
        {
            return Enumerable.Repeat<T?>(null, expectedCount).ToList();
        }

        EnsureCount("optional deliverable value list", list.Count, expectedCount);
        return list;
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

    private static int ParseRequiredInt(string? value, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new PluginApplicationException($"{fieldName} is required.");
        }

        if (!int.TryParse(value, out var parsed))
        {
            throw new PluginApplicationException($"{fieldName} must be a valid integer value.");
        }

        return parsed;
    }

    private static string ParseRequiredString(string? value, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new PluginApplicationException($"{fieldName} is required.");
        }

        return value.Trim();
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
