using Blackbird.Applications.Sdk.Common;

namespace Apps.Traduno.Models.Dtos;

public class ProjectDto
{
    [Display("Project ID")]
    public string Id { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public string? Name { get; set; }

    [Display("PO number")]
    public string? PoNumber { get; set; }

    [Display("Created by")]
    public string CreatedBy { get; set; } = string.Empty;

    [Display("Created at")]
    public DateTime CreatedAt { get; set; }

    public MoneyDto? Cost { get; set; }

    public IEnumerable<ProjectDeliverableDto> Deliverables { get; set; } = [];

    [Display("Delivered files")]
    public IEnumerable<DeliveredFileDto>? DeliveredFiles { get; set; }
}
