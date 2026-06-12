using Apps.Traduno.Handlers;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.Sdk.Common.Invocation;
using Microsoft.Extensions.Configuration;

namespace Tests.Traduno.Base;
public class TestBase
{
    public IEnumerable<AuthenticationCredentialsProvider> Creds { get; set; }

    public InvocationContext InvocationContext { get; set; }

    public FileManager FileManager { get; set; }

    public TestBase()
    {
        var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        Creds = config.GetSection("ConnectionDefinition").GetChildren()
            .Select(x => new AuthenticationCredentialsProvider(x.Key, x.Value ?? string.Empty))
            .ToList();

        InvocationContext = new InvocationContext
        {
            AuthenticationCredentialsProviders = Creds,
        };

        FileManager = new FileManager();
    }

    protected FileReference CreateSampleFileReference(string fileName = "sample-source.txt", string contentType = "text/plain")
    {
        return new FileReference
        {
            Name = fileName,
            ContentType = contentType
        };
    }

    protected async Task<(string ServiceCode, string SourceLanguageCode, string TargetLanguageCode)>
        GetDeliverableDataAsync()
    {
        var serviceHandler = new ServiceDataHandler(InvocationContext);
        var languageHandler = new LanguageDataHandler(InvocationContext);

        var service = (await serviceHandler.GetDataAsync(new DataSourceContext(), CancellationToken.None)).FirstOrDefault();
        var languages = (await languageHandler.GetDataAsync(new DataSourceContext(), CancellationToken.None))
            .Take(2)
            .ToArray();

        Assert.IsNotNull(service, "No services are available for the configured Traduno account.");
        Assert.IsTrue(languages.Any(), "No languages are available for the configured Traduno account.");

        var sourceLanguage = languages[0].Value;
        var targetLanguage = languages.Length > 1 ? languages[1].Value : languages[0].Value;

        return (service!.Value, sourceLanguage, targetLanguage);
    }
}
