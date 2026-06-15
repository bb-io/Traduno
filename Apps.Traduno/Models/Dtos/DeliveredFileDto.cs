using Blackbird.Applications.Sdk.Common;

namespace Apps.Traduno.Models.Dtos;

public class DeliveredFileDto
{
    [Display("File ID")]
    public string FileId { get; set; } = string.Empty;

    [Display("Download URL")]
    public string DownloadUrl { get; set; } = string.Empty;
}
