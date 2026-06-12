using Apps.Traduno.Constants;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Utils.Extensions.Sdk;
using Blackbird.Applications.Sdk.Utils.RestSharp;
using RestSharp;

namespace Apps.Traduno.Api;

public class TradunoRequest(string resource, Method method,
    IEnumerable<AuthenticationCredentialsProvider> credentialsProviders)
    : BlackBirdRestRequest(resource, method, credentialsProviders)
{
    protected override void AddAuth(IEnumerable<AuthenticationCredentialsProvider> creds)
    {
        var token = creds.Get(CredsNames.Token).Value;
        this.AddHeader("Authorization", $"Bearer {token}");
    }
}
