using Apps.Traduno.Handlers.Static;
using Apps.Traduno.Models.Identifiers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Traduno.Models.Requests;

public class OnProjectStatusChangedInput : ProjectIdentifier
{
    [Display("Status")]
    [StaticDataSource(typeof(ProjectStatusDataHandler))]
    public string? Status { get; set; }
}
