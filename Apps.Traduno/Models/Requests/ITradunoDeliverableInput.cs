namespace Apps.Traduno.Models.Requests;

public interface ITradunoDeliverableInput
{
    IEnumerable<string> DeliverableServiceCodeGroups { get; }

    IEnumerable<string>? DeliverableSourceLanguageCodes { get; }

    IEnumerable<string>? DeliverableTargetLanguageCodeGroups { get; }

    IEnumerable<string> DeliverableSchedulingModes { get; }

    IEnumerable<DateTime?>? DeliverableDeadlines { get; }

    IEnumerable<int?>? DeliverableTurnaroundTimes { get; }

    IEnumerable<string>? DeliverableDescriptions { get; }
}
