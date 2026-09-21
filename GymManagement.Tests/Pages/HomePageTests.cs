using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GymManagement;
using GymManagement.Web.Components.Pages;
using GymManagement.Web.Services;

namespace GymManagement.Tests.Pages;

// Smoke tests for the Home (Dashboard) page.
// Uses bUnit to render the real Blazor component and inspect what the DOM ends
// up looking like, so we're exercising the UI wiring - not just the domain logic.
[TestClass]
public class HomePageTests
{
    private static Bunit.TestContext CreateContext(GymDataStore store)
    {
        var ctx = new Bunit.TestContext();
        ctx.Services.AddSingleton(store);
        ctx.Services.AddSingleton<ReportingService>();
        return ctx;
    }

    [TestMethod]
    public void Home_RendersDashboardHeading()
    {
        using var ctx = CreateContext(new GymDataStore());

        var page = ctx.RenderComponent<Home>();

        var heading = page.Find("h1");
        Assert.AreEqual("ProGym Dashboard", heading.TextContent.Trim());
    }

    [TestMethod]
    public void Home_WithNoMembers_ShowsZeroActiveAndExpired()
    {
        using var ctx = CreateContext(new GymDataStore());

        var page = ctx.RenderComponent<Home>();

        // Both summary numbers should render as 0 when there's no membership data.
        var numbers = page.FindAll("p.card-text");
        Assert.IsTrue(numbers.Count >= 3, "expected 3 summary cards");
        Assert.AreEqual("0", numbers[0].TextContent.Trim()); // Active
        Assert.AreEqual("0", numbers[1].TextContent.Trim()); // Expired
        Assert.AreEqual("0", numbers[2].TextContent.Trim()); // Total check-ins
    }

    [TestMethod]
    public void Home_WithSeededClasses_ShowsUtilisationRows()
    {
        // The default GymDataStore seeds Yoga + Spin.
        using var ctx = CreateContext(new GymDataStore());

        var page = ctx.RenderComponent<Home>();

        var rows = page.FindAll("table tbody tr");
        Assert.AreEqual(2, rows.Count);
        Assert.IsTrue(page.Markup.Contains("Yoga"));
        Assert.IsTrue(page.Markup.Contains("Spin"));
    }
}
