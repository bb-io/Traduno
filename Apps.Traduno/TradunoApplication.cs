using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Metadata;

namespace Apps.Traduno;

public class TradunoApplication : IApplication, ICategoryProvider
{
    public IEnumerable<ApplicationCategory> Categories
    {
        get => [];
        set { }
    }

    public T GetInstance<T>()
    {
        throw new NotImplementedException();
    }
}
