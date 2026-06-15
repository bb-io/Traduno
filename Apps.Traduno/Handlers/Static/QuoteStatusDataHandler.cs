using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Traduno.Handlers.Static;

public class QuoteStatusDataHandler : IStaticDataSourceItemHandler
{
    public IEnumerable<DataSourceItem> GetData()
        => [
            new("pending", "Pending"),
            new("converted", "Converted"),
            new("cancelled", "Cancelled"),
            new("rejected", "Rejected"),
            new("estimated", "Estimated"),
            new("offer_expired", "Offer expired")
        ];
}
