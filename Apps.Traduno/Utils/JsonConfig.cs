using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Apps.Traduno.Utils;

public static class JsonConfig
{
    public static JsonSerializerSettings Settings => new()
    {
        ContractResolver = new DefaultContractResolver
        {
            NamingStrategy = new SnakeCaseNamingStrategy()
        },
        NullValueHandling = NullValueHandling.Ignore
    };
}
