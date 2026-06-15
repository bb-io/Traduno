using Apps.Traduno.Actions;
using Apps.Traduno.Models.Identifiers;
using Apps.Traduno.Models.Requests;
using Tests.Traduno.Base;

namespace Tests.Traduno;

[TestClass]
public class InvoiceActionTests : TestBase
{
    [TestMethod]
    public async Task Search_invoices_returns_collection()
    {
        var actions = new InvoiceActions(InvocationContext);

        var result = await actions.SearchInvoices(new SearchInvoicesInput());

        foreach (var invoice in result.Invoices)
        {
            Console.WriteLine($"Invoice: {invoice.Name} - {invoice.Id} - Status: {invoice.Status}");
        }

        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task Get_invoice_returns_invoice_when_any_invoice_exists()
    {
        var actions = new InvoiceActions(InvocationContext);
        var searchResult = await actions.SearchInvoices(new SearchInvoicesInput());
        var invoice = searchResult.Invoices.FirstOrDefault();

        if (invoice == null)
        {
            Assert.Inconclusive("No invoices are available for the configured Traduno account.");
            return;
        }

        var result = await actions.GetInvoice(new InvoiceIdentifier { InvoiceId = invoice.Id });

        Console.WriteLine($"Invoice: {result.Name} - {result.Id} - Status: {result.Status}");
        Assert.AreEqual(invoice.Id, result.Id);
    }
}
