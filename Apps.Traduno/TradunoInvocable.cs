using Apps.Traduno.Api;
using Apps.Traduno.Models.Dtos;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Traduno;

public class TradunoInvocable : BaseInvocable
{
    protected AuthenticationCredentialsProvider[] Creds =>
        InvocationContext.AuthenticationCredentialsProviders.ToArray();

    protected TradunoClient Client { get; }
    public TradunoInvocable(InvocationContext invocationContext) : base(invocationContext)
    {
        Client = new(Creds);
    }

    protected async Task<IReadOnlyList<string>> StageFilesAsync(IEnumerable<FileReference>? files,
        IFileManagementClient fileManagementClient)
    {
        if (files == null)
        {
            return [];
        }

        var stagedFileIds = new List<string>();
        foreach (var fileReference in files.Where(x => x != null))
        {
            var file = await fileManagementClient.DownloadAsync(fileReference);

            var request = new TradunoRequest("/files", Method.Post, Creds);
            request.AlwaysMultipartFormData = true;
            request.AddFile("file", () => file, fileReference.Name);

            var stagedFile = await Client.ExecuteWithErrorHandling<StagedFileDto>(request);
            stagedFileIds.Add(stagedFile.Id);
        }

        return stagedFileIds;
    }
}
