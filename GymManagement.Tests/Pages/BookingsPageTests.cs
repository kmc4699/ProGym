using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GymManagement;
using GymManagement.Web.Components.Pages;
using GymManagement.Web.Services;

namespace GymManagement.Tests.Pages;

// Automated UI tests for the Bookings page - book through the actual form,
// cancel through the actual button, and check what the user actually sees.
[TestClass]
public class BookingsPageTests
{
    private static (Bunit.TestContext ctx, GymDataStore store) CreateContext()
    {
        var store = new GymDataStore();
        // seed a member so the dropdown has a real option
        store.Members.Add(new Membership("M001", "Aroha Smith", DateTime.Today.AddMonths(6)));

        var ctx = new Bunit.TestContext();
        ctx.Services.AddSingleton(store);
        ctx.Services.AddSingleton<BookingService>();
        return (ctx, store);
    }

    [TestMethod]
    public void Bookings_InitiallyShowsEmptyState()
    {
        var (ctx, _) = CreateContext();
        using var _ctx = ctx;

        var page = ctx.RenderComponent<Bookings>();

        Assert.IsTrue(page.Markup.Contains("No bookings yet"));
    }

    [TestMethod]
    public void Bookings_SelectingMemberAndClass_AndSubmitting_BooksSuccessfully()
    {
        var (ctx, store) = CreateContext();
        using var _ctx = ctx;
        var page = ctx.RenderComponent<Bookings>();

        // The user picks a member and a class. Re-find after each change because
        // Blazor re-renders and the previous event handler ids become stale.
        page.FindAll("select")[0].Change("M001");
        page.FindAll("select")[1].Change(store.Classes[0].Id);

        page.Find("form").Submit();

        var alert = page.Find("div.alert-success");
        Assert.IsTrue(alert.TextContent.Contains("Booking confirmed"));
        Assert.AreEqual(1, store.Bookings.Count);
        Assert.IsTrue(page.Markup.Contains("Aroha Smith"));
    }

    [TestMethod]
    public void Bookings_SubmittingWithoutSelection_ShowsErrorMessage()
    {
        var (ctx, _) = CreateContext();
        using var _ctx = ctx;
        var page = ctx.RenderComponent<Bookings>();

        // Submit without touching the dropdowns.
        page.Find("form").Submit();

        var alert = page.Find("div.alert-danger");
        Assert.IsTrue(alert.TextContent.Contains("Select"));
    }

    [TestMethod]
    public void Bookings_CancellingAnActiveBooking_ReleasesTheSlotAndShowsCancelledBadge()
    {
        var (ctx, store) = CreateContext();
        using var _ctx = ctx;
        var page = ctx.RenderComponent<Bookings>();

        // Book first (re-find selects after each change to avoid stale handler ids)
        page.FindAll("select")[0].Change("M001");
        page.FindAll("select")[1].Change(store.Classes[0].Id);
        page.Find("form").Submit();

        // Now click the Cancel button in the row.
        page.Find("button.btn-outline-danger").Click();

        // The cancellation confirmation shows and the status badge flips to "Cancelled".
        var alert = page.Find("div.alert-success");
        Assert.IsTrue(alert.TextContent.Contains("cancelled"));
        Assert.IsTrue(page.Markup.Contains("Cancelled"));
        Assert.IsTrue(store.Bookings[0].IsCancelled);
    }
}
