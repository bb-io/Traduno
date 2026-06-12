using Apps.Traduno.Api;
using Apps.Traduno.Models.Identifiers;
using Apps.Traduno.Models.Responses;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;
using System.Net.Mime;

namespace Apps.Traduno.Actions;

[ActionList("Files")]
public class FileActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient)
    : TradunoInvocable(invocationContext)
{
    [Action("Download delivered file", Description = "Download a delivered project file.")]
    public async Task<FileResponse> DownloadDeliveredFile([ActionParameter] DeliveredFileIdentifier input)
    {
        var request = new TradunoRequest($"/projects/{input.ProjectId}/delivered_files/{input.FileId}", Method.Get, Creds);
        var response = await Client.ExecuteWithErrorHandling(request);
        return new FileResponse
        {
            File = await UploadResponseAsFileAsync(response, $"{input.ProjectId}-{input.FileId}")
        };
    }

    [Action("Download invoice document", Description = "Download an invoice PDF document.")]
    public async Task<FileResponse> DownloadInvoiceDocument([ActionParameter] InvoiceIdentifier input)
    {
        var request = new TradunoRequest($"/invoices/{input.InvoiceId}/document", Method.Get, Creds);
        var response = await Client.ExecuteWithErrorHandling(request);
        return new FileResponse
        {
            File = await UploadResponseAsFileAsync(response, $"{input.InvoiceId}.pdf")
        };
    }

    private async Task<FileReference> UploadResponseAsFileAsync(RestResponse response, string fallbackFileName)
    {
        if (response.RawBytes == null || response.RawBytes.Length == 0)
        {
            throw new PluginApplicationException("Traduno returned an empty file response.");
        }

        var fileName = fallbackFileName;
        var contentDispositionHeader = response.ContentHeaders == null
            ? null
            : response.ContentHeaders
                .FirstOrDefault(x => x.Name == "Content-Disposition")
                ?.Value
                ?.ToString();
        if (!string.IsNullOrWhiteSpace(contentDispositionHeader))
        {
            fileName = new ContentDisposition(contentDispositionHeader).FileName ?? fallbackFileName;
        }

        using var stream = new MemoryStream(response.RawBytes);
        return await fileManagementClient.UploadAsync(stream, response.ContentType ?? "application/octet-stream", fileName);
    }
}
