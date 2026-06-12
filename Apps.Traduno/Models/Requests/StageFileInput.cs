using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Traduno.Models.Requests;

public class StageFileInput
{
    public FileReference File { get; set; } = default!;
}
