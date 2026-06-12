using Apps.Traduno.Constants;
using Apps.Traduno.Models.Api;
using Apps.Traduno.Models.Dtos;
using Apps.Traduno.Utils;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Utils.Extensions.Sdk;
using Blackbird.Applications.Sdk.Utils.Extensions.String;
using Blackbird.Applications.Sdk.Utils.RestSharp;
using Newtonsoft.Json;
using Polly;
using RestSharp;
using System.Net;

namespace Apps.Traduno.Api;

public class TradunoClient : BlackBirdRestClient
{
    private const int PageSize = 100;

    private readonly AuthenticationCredentialsProvider[] _creds;
    private readonly ResiliencePipeline<RestResponse> _retryPolicy;

    public TradunoClient(IEnumerable<AuthenticationCredentialsProvider> creds) : base(new()
    {
        BaseUrl = BuildBaseUrl(creds),
        Timeout = TimeSpan.FromMinutes(3)
    })
    {
        _creds = creds.ToArray();
        _retryPolicy = TradunoPollyPolicies.GetTooManyRequestsRetryPolicy();
    }

    protected override JsonSerializerSettings? JsonSettings => JsonConfig.Settings;

    public async Task<AccountDto> GetAccountAsync(CancellationToken cancellationToken = default)
    {
        var request = new TradunoRequest("/account", Method.Get, _creds);
        return await ExecuteWithErrorHandling<AccountDto>(request, cancellationToken);
    }

    public override async Task<T> ExecuteWithErrorHandling<T>(RestRequest request)
    {
        string content = (await ExecuteWithErrorHandling(request)).Content;
        T val = JsonConvert.DeserializeObject<T>(content, JsonSettings);
        if (val == null)
        {
            throw new Exception($"Could not parse {content} to {typeof(T)}");
        }

        return val;
    }

    public override async Task<RestResponse> ExecuteWithErrorHandling(RestRequest request)
    {
        RestResponse restResponse = await ExecuteAsync(request);
        if (!restResponse.IsSuccessStatusCode)
        {
            throw ConfigureErrorException(restResponse);
        }

        return restResponse;
    }

    public async Task<IReadOnlyList<T>> GetAllPaginatedAsync<T>(string resource,
        IDictionary<string, string?>? queryParameters = null,
        CancellationToken cancellationToken = default)
    {
        var page = 1;
        var results = new List<T>();
        PaginatedResponse<T>? response;

        do
        {
            var request = new TradunoRequest(resource, Method.Get, _creds);
            AddQueryParameters(request, queryParameters);
            request.AddQueryParameter("page", page.ToString());
            request.AddQueryParameter("limit", PageSize.ToString());

            response = await ExecuteWithErrorHandling<PaginatedResponse<T>>(request, cancellationToken);
            if (response.Items != null)
            {
                results.AddRange(response.Items);
            }

            page++;
        } while (response != null && page <= response.TotalPages);

        return results;
    }

    public async Task<RestResponse> ExecuteWithErrorHandling(RestRequest request, CancellationToken cancellationToken)
    {
        var response = await _retryPolicy.ExecuteAsync(async _ => await ExecuteAsync(request, cancellationToken));
        if (!response.IsSuccessStatusCode)
        {
            throw ConfigureErrorException(response);
        }

        return response;
    }

    public async Task<T> ExecuteWithErrorHandling<T>(RestRequest request, CancellationToken cancellationToken)
    {
        var content = (await ExecuteWithErrorHandling(request, cancellationToken)).Content;
        if (string.IsNullOrWhiteSpace(content))
        {
            throw new PluginApplicationException($"Received empty response when expecting {typeof(T).Name}");
        }

        var result = JsonConvert.DeserializeObject<T>(content, JsonSettings);
        if (result == null)
        {
            throw new PluginApplicationException($"Could not parse {content} to {typeof(T).Name}");
        }

        return result;
    }

    protected override Exception ConfigureErrorException(RestResponse response)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(response.Content) &&
                response.Content.TrimStart().StartsWith("<html", StringComparison.OrdinalIgnoreCase))
            {
                return new PluginApplicationException($"Traduno returned HTML instead of JSON: {response.Content}");
            }

            var error = JsonConvert.DeserializeObject<ErrorResponse>(response.Content ?? string.Empty, JsonSettings);
            if (error != null && !string.IsNullOrWhiteSpace(error.ErrorMessage))
            {
                return new PluginApplicationException($"{error.ErrorMessage} ({error.ErrorCode})");
            }
        }
        catch (JsonException)
        {
        }

        return new PluginApplicationException(response.Content ?? response.ErrorMessage ?? response.StatusCode.ToString());
    }

    private static Uri BuildBaseUrl(IEnumerable<AuthenticationCredentialsProvider> creds)
    {
        var host = creds.Get(CredsNames.Host).Value?.Trim() ?? throw new PluginApplicationException("Host is missing");
        if (!host.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
            !host.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            host = $"https://{host}";
        }

        var uri = new Uri(host);
        var builder = new UriBuilder(uri)
        {
            Scheme = Uri.UriSchemeHttps,
            Path = "/api/customer-portal/v1"
        };

        return builder.Uri;
    }

    private static void AddQueryParameters(RestRequest request, IDictionary<string, string?>? queryParameters)
    {
        if (queryParameters == null)
        {
            return;
        }

        foreach (var queryParameter in queryParameters.Where(x => !string.IsNullOrWhiteSpace(x.Value)))
        {
            request.AddQueryParameter(queryParameter.Key, queryParameter.Value!);
        }
    }
}
