using Apps.Traduno.Actions;
using Apps.Traduno.Models.Identifiers;
using Apps.Traduno.Models.Requests;
using Tests.Traduno.Base;

namespace Tests.Traduno;

[TestClass]
public class FileActionTests : TestBase
{
    [TestMethod]
    public async Task Download_delivered_file_returns_file_when_available()
    {
        var projectActions = new ProjectActions(InvocationContext, FileManager);
        var fileActions = new FileActions(InvocationContext, FileManager);

        var deliveredProjects = await projectActions.SearchProjects(new SearchProjectsInput
        {
            Status = "delivered",
            ShowAll = true
        });

        foreach (var project in deliveredProjects.Projects)
        {
            var projectDetails = await projectActions.GetProject(new ProjectIdentifier { ProjectId = project.Id });
            var deliveredFile = projectDetails.DeliveredFiles?.FirstOrDefault();
            if (deliveredFile == null)
            {
                continue;
            }

            var result = await fileActions.DownloadDeliveredFile(new DeliveredFileIdentifier
            {
                ProjectId = project.Id,
                FileId = deliveredFile.FileId
            });

            Console.WriteLine($"Downloaded delivered file: {result.File.Name}");
            Assert.IsFalse(string.IsNullOrWhiteSpace(result.File.Name));
            return;
        }

        Assert.Inconclusive("No delivered project with downloadable files is available for the configured Traduno account.");
    }

    [TestMethod]
    public async Task Download_invoice_document_returns_file_when_available()
    {
        var invoiceActions = new InvoiceActions(InvocationContext);
        var fileActions = new FileActions(InvocationContext, FileManager);

        var invoices = await invoiceActions.SearchInvoices(new SearchInvoicesInput());
        var invoice = invoices.Invoices.FirstOrDefault();

        if (invoice == null)
        {
            Assert.Inconclusive("No invoices are available for the configured Traduno account.");
            return;
        }

        var result = await fileActions.DownloadInvoiceDocument(new InvoiceIdentifier
        {
            InvoiceId = invoice.Id
        });

        Console.WriteLine($"Downloaded invoice file: {result.File.Name}");
        Assert.IsFalse(string.IsNullOrWhiteSpace(result.File.Name));
    }
}
