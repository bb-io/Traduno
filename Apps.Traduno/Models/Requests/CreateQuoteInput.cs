using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Traduno.Models.Requests;

public class CreateQuoteInput : CreateTradunoEntityInputBase
{
    [Display("Source files")]
    public IEnumerable<FileReference>? SourceFiles { get; set; }
}
