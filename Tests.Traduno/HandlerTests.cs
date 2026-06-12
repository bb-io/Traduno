using Apps.Traduno.Actions;
using Apps.Traduno.Handlers;
using Apps.Traduno.Handlers.Static;
using Apps.Traduno.Models.Identifiers;
using Apps.Traduno.Models.Requests;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Tests.Traduno.Base;

namespace Tests.Traduno;

[TestClass]
public class HandlerTests : TestBase
{
    [TestMethod]
    public async Task Language_data_handler_returns_values()
    {
        await AssertHandlerReturnsValues(new LanguageDataHandler(InvocationContext), nameof(LanguageDataHandler));
    }

    [TestMethod]
    public async Task Service_data_handler_returns_values()
    {
        await AssertHandlerReturnsValues(new ServiceDataHandler(InvocationContext), nameof(ServiceDataHandler));
    }

    [TestMethod]
    public async Task Translation_area_data_handler_returns_values()
    {
        await AssertHandlerReturnsValues(new TranslationAreaDataHandler(InvocationContext), nameof(TranslationAreaDataHandler));
    }

    [TestMethod]
    public async Task Unit_type_data_handler_returns_values()
    {
        await AssertHandlerReturnsValues(new UnitTypeDataHandler(InvocationContext), nameof(UnitTypeDataHandler));
    }

    [TestMethod]
    public async Task Currency_data_handler_returns_values()
    {
        await AssertHandlerReturnsValues(new CurrencyDataHandler(InvocationContext), nameof(CurrencyDataHandler));
    }

    [TestMethod]
    public async Task Billing_entity_data_handler_returns_values()
    {
        await AssertHandlerReturnsValues(new BillingEntityDataHandler(InvocationContext), nameof(BillingEntityDataHandler));
    }

    [TestMethod]
    public async Task Project_data_handler_returns_collection()
    {
        await AssertHandlerReturnsCollection(new ProjectDataHandler(InvocationContext), nameof(ProjectDataHandler));
    }

    [TestMethod]
    public async Task Quote_data_handler_returns_collection()
    {
        await AssertHandlerReturnsCollection(new QuoteDataHandler(InvocationContext), nameof(QuoteDataHandler));
    }

    [TestMethod]
    public async Task Invoice_data_handler_returns_collection()
    {
        await AssertHandlerReturnsCollection(new InvoiceDataHandler(InvocationContext), nameof(InvoiceDataHandler));
    }

    [TestMethod]
    public async Task Language_handler_applies_search_filter()
    {
        var handler = new LanguageDataHandler(InvocationContext);
        var allItems = await handler.GetDataAsync(new DataSourceContext(), CancellationToken.None);
        var filteredItems = await handler.GetDataAsync(new DataSourceContext { SearchString = "en" }, CancellationToken.None);

        Console.WriteLine($"Languages total: {allItems.Count()}");
        Console.WriteLine($"Languages filtered: {filteredItems.Count()}");

        Assert.IsTrue(allItems.Any(), "Expected languages handler to return values.");
        Assert.IsTrue(filteredItems.Any(), "Expected filtered language handler to return values for search 'en'.");
        Assert.IsTrue(filteredItems.All(x =>
            x.DisplayName.Contains("en", StringComparison.OrdinalIgnoreCase) ||
            x.Value.Contains("en", StringComparison.OrdinalIgnoreCase)));
    }

    [TestMethod]
    public async Task Delivered_file_handler_returns_values_for_delivered_project_when_available()
    {
        var actions = new ProjectActions(InvocationContext);
        var deliveredProjects = await actions.SearchProjects(new SearchProjectsInput
        {
            Status = "delivered",
            ShowAll = true
        });

        foreach (var project in deliveredProjects.Projects)
        {
            var projectDetails = await actions.GetProject(new ProjectIdentifier { ProjectId = project.Id });
            if (projectDetails.DeliveredFiles == null || !projectDetails.DeliveredFiles.Any())
            {
                continue;
            }

            var handler = new DeliveredFileDataHandler(InvocationContext,
                new ProjectIdentifier { ProjectId = project.Id });

            var result = await handler.GetDataAsync(new DataSourceContext(), CancellationToken.None);
            WriteItems(nameof(DeliveredFileDataHandler), result);

            Assert.IsTrue(result.Any(), "Expected delivered file handler to return at least one file.");
            return;
        }

        Assert.Inconclusive("No delivered project with visible delivered files is available for the configured Traduno account.");
    }

    [TestMethod]
    public void Project_status_data_handler_returns_values()
    {
        AssertStaticHandlerReturnsValues(new ProjectStatusDataHandler(), nameof(ProjectStatusDataHandler));
    }

    [TestMethod]
    public void Quote_status_data_handler_returns_values()
    {
        AssertStaticHandlerReturnsValues(new QuoteStatusDataHandler(), nameof(QuoteStatusDataHandler));
    }

    [TestMethod]
    public void Invoice_status_data_handler_returns_values()
    {
        AssertStaticHandlerReturnsValues(new InvoiceStatusDataHandler(), nameof(InvoiceStatusDataHandler));
    }

    [TestMethod]
    public void Deliverable_scheduling_mode_data_handler_returns_values()
    {
        AssertStaticHandlerReturnsValues(new DeliverableSchedulingModeDataHandler(), nameof(DeliverableSchedulingModeDataHandler));
    }

    private async Task AssertHandlerReturnsValues(IAsyncDataSourceItemHandler handler, string handlerName)
    {
        var result = await handler.GetDataAsync(new DataSourceContext(), CancellationToken.None);
        WriteItems(handlerName, result);

        Assert.IsTrue(result.Any(), $"{handlerName} should return at least one item.");
    }

    private async Task AssertHandlerReturnsCollection(IAsyncDataSourceItemHandler handler, string handlerName)
    {
        var result = await handler.GetDataAsync(new DataSourceContext(), CancellationToken.None);
        WriteItems(handlerName, result);

        Assert.IsNotNull(result, $"{handlerName} should return a collection.");
    }

    private void AssertStaticHandlerReturnsValues(IStaticDataSourceItemHandler handler, string handlerName)
    {
        var result = handler.GetData().ToArray();
        WriteItems(handlerName, result);

        Assert.IsTrue(result.Any(), $"{handlerName} should return at least one item.");
    }

    private static void WriteItems(string handlerName, IEnumerable<DataSourceItem> items)
    {
        var dataSourceItems = items.ToArray();
        Console.WriteLine($"{handlerName} total: {dataSourceItems.Length}");
        foreach (var item in dataSourceItems)
        {
            Console.WriteLine($"{item.Value}: {item.DisplayName}");
        }
    }
}
