using Blackbird.Applications.Sdk.Common;

namespace Apps.Traduno.Models.Dtos;

public class BillingEntityDto
{
    public string Id { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    [Display("Default")]
    public bool Default { get; set; }
}
