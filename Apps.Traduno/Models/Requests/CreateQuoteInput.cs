namespace Apps.Traduno.Models.Requests;

public class CreateQuoteInput : CreateTradunoEntityInputBase
{
    public IEnumerable<string>? SourceFileIds { get; set; }
}
