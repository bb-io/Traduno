using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Traduno.Handlers.Static;

public class ProjectStatusDataHandler : IStaticDataSourceItemHandler
{
    public IEnumerable<DataSourceItem> GetData()
        => [
            new("in_progress", "In progress"),
            new("on_hold", "On hold"),
            new("delivered", "Delivered"),
            new("in_review", "In review"),
            new("cancelled", "Cancelled")
        ];
}
