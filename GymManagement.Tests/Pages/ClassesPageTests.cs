using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GymManagement;
using GymManagement.Web.Components.Pages;
using GymManagement.Web.Services;

namespace GymManagement.Tests.Pages;

// Component tests for the Classes page.
[TestClass]
public class ClassesPageTests
{
    private static (Bunit.TestContext ctx, GymDataStore store) CreateContext()
    {
        var store = new GymDataStore();
        var tempFile = Path.Combine(Path.GetTempPath(), $"progym-classes-{Guid.NewGuid():N}.json");

        var ctx = new Bunit.TestContext();
        ctx.Services.AddSingleton(store);
        ctx.Services.AddSingleton(new PersistenceService(tempFile));
        return (ctx, store);
    }

    [TestMethod]
    public void Classes_InitiallyShowsSeededClasses()
    {
        var (ctx, _) = CreateContext();
        using var _ctx = ctx;

        var page = ctx.RenderComponent<Classes>();

        // The seeded Yoga + Spin classes should render as table rows.
        var rows = page.FindAll("table tbody tr");
        Assert.AreEqual(2, rows.Count);
        Assert.IsTrue(page.Markup.Contains("Yoga"));
        Assert.IsTrue(page.Markup.Contains("Spin"));
    }

    [TestMethod]
    public void Classes_AddValidClass_AppendsRowAndShowsSuccess()
    {
        var (ctx, store) = CreateContext();
        using var _ctx = ctx;
        var page = ctx.RenderComponent<Classes>();

        page.FindAll("input")[0].Change("C10");                      // Class ID
        page.FindAll("input")[1].Change("Pilates");                  // Name
        page.FindAll("input[type=date]")[0]
            .Change(DateTime.Today.AddDays(3).ToString("yyyy-MM-dd"));
        page.FindAll("input[type=number]")[0].Change("15");          // Capacity

        page.Find("form").Submit();

        Assert.AreEqual(3, store.Classes.Count);
        Assert.IsTrue(page.Markup.Contains("Pilates"));
        Assert.IsTrue(page.Find("div.alert-success").TextContent.Contains("Added"));
    }

    [TestMethod]
    public void Classes_AddInvalidClass_ZeroCapacity_ShowsErrorAndDoesNotAdd()
    {
        var (ctx, store) = CreateContext();
        using var _ctx = ctx;
        int classesBefore = store.Classes.Count;
        var page = ctx.RenderComponent<Classes>();

        page.FindAll("input")[0].Change("C10");
        page.FindAll("input")[1].Change("Broken");
        page.FindAll("input[type=date]")[0]
            .Change(DateTime.Today.AddDays(3).ToString("yyyy-MM-dd"));
        page.FindAll("input[type=number]")[0].Change("0");           // invalid

        page.Find("form").Submit();

        Assert.AreEqual(classesBefore, store.Classes.Count);
        Assert.IsNotNull(page.Find("div.alert-danger"));
    }

    // Feature F3: filter box narrows the visible class list live.
    [TestMethod]
    public void Classes_FilterByName_ShowsOnlyMatchingRows()
    {
        var (ctx, _) = CreateContext();
        using var _ctx = ctx;
        var page = ctx.RenderComponent<Classes>();

        // Both seeded classes are visible before filtering.
        Assert.AreEqual(2, page.FindAll("table tbody tr").Count);

        // Type "spin" - only Spin should remain.
        page.Find("input[type=text]").Input("spin");

        var rows = page.FindAll("table tbody tr");
        Assert.AreEqual(1, rows.Count);
        Assert.IsTrue(rows[0].TextContent.Contains("Spin"));
    }

    // Feature F3: no-match message when the filter excludes everything.
    [TestMethod]
    public void Classes_FilterWithNoMatches_ShowsNoResultsMessage()
    {
        var (ctx, _) = CreateContext();
        using var _ctx = ctx;
        var page = ctx.RenderComponent<Classes>();

        page.Find("input[type=text]").Input("zzzzz");

        Assert.AreEqual(0, page.FindAll("table tbody tr").Count);
        Assert.IsTrue(page.Markup.Contains("No classes match your filter"));
    }

    // Feature F3: the "hide past classes" checkbox removes classes with a past start time.
    [TestMethod]
    public void Classes_HidePastCheckbox_ExcludesClassesInThePast()
    {
        var (ctx, store) = CreateContext();
        using var _ctx = ctx;

        // Add a class that already started (past start time)
        store.Classes.Add(new FitnessClass("C99", "Yesterday Class",
            DateTime.Now.AddHours(-2), 5));
        var page = ctx.RenderComponent<Classes>();

        Assert.AreEqual(3, page.FindAll("table tbody tr").Count); // 2 seeded + 1 past

        // Tick the "hide past classes" checkbox
        page.Find("input[type=checkbox]").Change(true);

        var rows = page.FindAll("table tbody tr");
        Assert.AreEqual(2, rows.Count);
        Assert.IsFalse(page.Markup.Contains("Yesterday Class"));
    }
}
