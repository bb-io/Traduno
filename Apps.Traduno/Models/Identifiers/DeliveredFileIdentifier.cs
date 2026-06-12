using Apps.Traduno.Handlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Traduno.Models.Identifiers;

public class DeliveredFileIdentifier
{
    [Display("Project ID")]
    [DataSource(typeof(ProjectDataHandler))]
    public string ProjectId { get; set; } = string.Empty;

    [Display("File ID")]
    [DataSource(typeof(DeliveredFileDataHandler))]
    public string FileId { get; set; } = string.Empty;
}
