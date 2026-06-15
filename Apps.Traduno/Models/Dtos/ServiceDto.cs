using Blackbird.Applications.Sdk.Common;

namespace Apps.Traduno.Models.Dtos;

public class ServiceDto
{
    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    [Display("Language independent")]
    public bool LanguageIndependent { get; set; }
}
