using Apps.Traduno.Actions;
using Apps.Traduno.Models.Identifiers;
using Apps.Traduno.Models.Requests;
using Tests.Traduno.Base;

namespace Tests.Traduno;

[TestClass]
public class ProjectActionTests : TestBase
{
    [TestMethod]
    public async Task Search_projects_returns_collection()
    {
        var actions = new ProjectActions(InvocationContext, FileManager);

        var result = await actions.SearchProjects(new SearchProjectsInput());

        foreach (var project in result.Projects)
        {
            Console.WriteLine($"Name: {project.Name} - Id: {project.Id}");
        }

        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task Get_project_returns_project_when_any_project_exists()
    {
        var actions = new ProjectActions(InvocationContext, FileManager);

        var result = await actions.GetProject(new ProjectIdentifier { ProjectId = "3928015421" });

        Console.WriteLine($"Project: {result.Name} - {result.Id}");
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task Create_project_returns_created_project_when_account_allows_it()
    {
        var actions = new ProjectActions(InvocationContext, FileManager);
        var deliverableData = await GetDeliverableDataAsync();

        try
        {
            var uniqueSuffix = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var result = await actions.CreateProject(new CreateProjectInput
            {
                Name = $"BB test project {uniqueSuffix}",
                PoNumber = $"BB-{uniqueSuffix}",
                SourceFiles = [CreateSampleFileReference()],
                CurrencyId = "978",
                TranslationAreaId = "27",
                DeliveryFilesFormat = "docx",
                DeliverableServiceCodeGroups = [deliverableData.ServiceCode],
                DeliverableTargetLanguageCodeGroups = [deliverableData.TargetLanguageCode],
                DeliverableSchedulingModes = ["turnaround"],
                DeliverableTurnaroundTimes = [1]
            });

            Console.WriteLine($"Created project: {result.Name} - {result.Id}");
            Assert.IsFalse(string.IsNullOrWhiteSpace(result.Id));
        }
        catch (Exception ex) when (ex.Message.Contains("bypass approval", StringComparison.OrdinalIgnoreCase) ||
                                   ex.Message.Contains("not permitted", StringComparison.OrdinalIgnoreCase) ||
                                   ex.Message.Contains("forbidden", StringComparison.OrdinalIgnoreCase))
        {
            Assert.Inconclusive($"Project creation is not available for this Traduno account: {ex.Message}");
        }
    }
}
