using Blackbird.Applications.Sdk.Common;

namespace Apps.Traduno.Models.Dtos;

public class QuantityDto
{
    public decimal Value { get; set; }

    [Display("Unit type ID")]
    public int UnitTypeId { get; set; }

    [Display("Unit type name")]
    public string? UnitTypeName { get; set; }
}
