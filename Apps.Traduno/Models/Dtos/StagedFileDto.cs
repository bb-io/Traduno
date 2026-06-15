using Blackbird.Applications.Sdk.Common;

namespace Apps.Traduno.Models.Dtos;

public class StagedFileDto
{
    [Display("File ID")]
    public string Id { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    [Display("Size in bytes")]
    public long Size { get; set; }
}
