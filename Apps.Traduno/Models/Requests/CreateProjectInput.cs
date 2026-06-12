namespace Apps.Traduno.Models.Requests;

public class CreateProjectInput : CreateTradunoEntityInputBase
{
    public IEnumerable<string> SourceFileIds { get; set; } = [];
}
