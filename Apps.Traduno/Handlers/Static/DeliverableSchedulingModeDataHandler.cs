using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Traduno.Handlers.Static;

public class DeliverableSchedulingModeDataHandler : IStaticDataSourceItemHandler
{
    public IEnumerable<DataSourceItem> GetData()
        => [
            new("deadline", "Deadline"),
            new("turnaround", "Turnaround time")
        ];
}
