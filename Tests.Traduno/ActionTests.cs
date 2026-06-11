using Apps.Traduno.Actions;
using Tests.Traduno.Base;

namespace Tests.Traduno;

[TestClass]
public class ActionTests : TestBase
{
    [TestMethod]
    public async Task Dynamic_handler_works()
    {
        var actions = new Actions(InvocationContext);

        await actions.Action();
    }
}
