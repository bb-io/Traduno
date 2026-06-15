using Apps.Traduno.Api;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Connections;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Traduno.Connections;

public class ConnectionValidator(InvocationContext invocationContext) : BaseInvocable(invocationContext), IConnectionValidator
{
    public async ValueTask<ConnectionValidationResponse> ValidateConnection(
        IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
        CancellationToken cancellationToken)
    {
        try
        {
            var client = new TradunoClient(authenticationCredentialsProviders);
            await client.GetAccountAsync(cancellationToken);
        }
        catch (PluginApplicationException ex) when (IsValidationError(ex))
        {
            InvocationContext.Logger?.LogError($"Connection validation failed: {ex.Message}", []);

            return new()
            {
                IsValid = false,
                Message = ex.Message
            };
        }
        catch
        {
            return new()
            {
                IsValid = true
            };
        }

        return new()
        {
            IsValid = true
        };
    }

    private static bool IsValidationError(PluginApplicationException ex) =>
        ex.Message.Contains("400", StringComparison.OrdinalIgnoreCase) ||
        ex.Message.Contains("401", StringComparison.OrdinalIgnoreCase) ||
        ex.Message.Contains("403", StringComparison.OrdinalIgnoreCase) ||
        ex.Message.Contains("unauthorized", StringComparison.OrdinalIgnoreCase) ||
        ex.Message.Contains("forbidden", StringComparison.OrdinalIgnoreCase) ||
        ex.Message.Contains("host is missing", StringComparison.OrdinalIgnoreCase) ||
        ex.Message.Contains("invalid uri", StringComparison.OrdinalIgnoreCase);
}
