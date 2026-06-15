using Apps.Traduno.Models.Dtos;
using Blackbird.Applications.Sdk.Common;

namespace Apps.Traduno.Models.Responses;

public class SearchProjectsResponse
{
    public IEnumerable<ProjectDto> Projects { get; set; } = [];

    [Display("Project IDs")]
    public IEnumerable<string> ProjectIds => Projects.Select(x => x.Id);
}
