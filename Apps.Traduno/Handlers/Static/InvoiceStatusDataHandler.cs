using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Traduno.Handlers.Static;

public class InvoiceStatusDataHandler : IStaticDataSourceItemHandler
{
    public IEnumerable<DataSourceItem> GetData()
        => [
            new("cancelled", "Cancelled"),
            new("not_paid", "Not paid"),
            new("partially_paid", "Partially paid"),
            new("paid", "Paid")
        ];
}
