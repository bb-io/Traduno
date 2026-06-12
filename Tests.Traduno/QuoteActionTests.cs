using Apps.Traduno.Actions;
using Apps.Traduno.Models.Identifiers;
using Apps.Traduno.Models.Requests;
using Tests.Traduno.Base;

namespace Tests.Traduno;

[TestClass]
public class QuoteActionTests : TestBase
{
    [TestMethod]
    public async Task Search_quotes_returns_collection()
    {
        var actions = new QuoteActions(InvocationContext, FileManager);

        var result = await actions.SearchQuotes(new SearchQuotesInput());

        foreach (var quote in result.Quotes)
        {
            Console.WriteLine($"Name:{quote.Name} - Id: {quote.Id} - Status: {quote.Status}");
        }

        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task Get_quote_returns_quote_when_any_quote_exists()
    {
        var actions = new QuoteActions(InvocationContext, FileManager);

        var result = await actions.GetQuote(new QuoteIdentifier { QuoteId = "8520944231" });

        Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(result, Newtonsoft.Json.Formatting.Indented));
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task Create_quote_request_returns_created_quote()
    {
        var actions = new QuoteActions(InvocationContext, FileManager);
        var deliverableData = await GetDeliverableDataAsync();

        var result = await actions.CreateQuote(new CreateQuoteInput
        {
            Name = $"BB aa test quote {DateTime.UtcNow:yyyyMMddHHmmss}",
            DeliveryFilesFormat = "docx",
            DeliverableServiceCodeGroups = [deliverableData.ServiceCode],
            DeliverableSourceLanguageCodes = [deliverableData.SourceLanguageCode],
            DeliverableTargetLanguageCodeGroups = [deliverableData.TargetLanguageCode],
            DeliverableSchedulingModes = ["turnaround"],
            CurrencyId = "978",
            TranslationAreaId = "27",
            DeliverableTurnaroundTimes = [1],
            SourceFiles = [CreateSampleFileReference()]
        });

        Console.WriteLine($"Created quote: {result.Name} - {result.Id} - {result.Status}");
        Assert.IsFalse(string.IsNullOrWhiteSpace(result.Id));
    }

    [TestMethod]
    public async Task Reject_quote_returns_rejected_quote_for_new_quote()
    {
        var actions = new QuoteActions(InvocationContext, FileManager);
       
        var result = await actions.RejectQuote(new QuoteIdentifier { QuoteId = "8520944231" }, new RejectQuoteInput
        {
            RejectMessage = "Rejected by automated test"
        });

        Console.WriteLine($"Rejected quote: {result.Name} - {result.Id} - {result.Status}");
        Assert.AreEqual("rejected", result.Status, ignoreCase: true);
    }

    [TestMethod]
    public async Task Accept_quote_returns_converted_quote_when_estimated_quote_exists()
    {
        var actions = new QuoteActions(InvocationContext, FileManager);
        var result = await actions.AcceptQuote(new QuoteIdentifier { QuoteId = "8520944231" });

        Console.WriteLine($"Accepted quote: {result.Name} - {result.Id} - {result.Status}");
        Assert.AreEqual("converted", result.Status, ignoreCase: true);
    }
}
