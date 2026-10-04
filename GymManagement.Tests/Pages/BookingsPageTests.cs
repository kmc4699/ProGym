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
    private static (Bunit.BunitContext ctx, GymDataStore store) CreateContext()
    {
        var store = new GymDataStore();
        // seed a member so the dropdown has a real option
        store.Members.Add(new Membership("M001", "Aroha Smith", DateTime.Today.AddMonths(6)));

        // persistence points at a unique temp file so tests don't collide
        var tempFile = Path.Combine(Path.GetTempPath(), $"progym-bunit-{Guid.NewGuid():N}.json");

        var ctx = new Bunit.BunitContext();
        ctx.Services.AddSingleton(store);
        ctx.Services.AddSingleton<BookingService>();
        ctx.Services.AddSingleton<WaitlistService>();
        ctx.Services.AddSingleton(new PersistenceService(tempFile));
        return (ctx, store);
    }

    [TestMethod]
    public void Bookings_InitiallyShowsEmptyState()
    {
        var (ctx, _) = CreateContext();
        using var _ctx = ctx;

        var page = ctx.Render<Bookings>();

        Assert.IsTrue(page.Markup.Contains("No bookings yet"));
    }

    [TestMethod]
    public void Bookings_SelectingMemberAndClass_AndSubmitting_BooksSuccessfully()
    {
        var (ctx, store) = CreateContext();
        using var _ctx = ctx;
        var page = ctx.Render<Bookings>();

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
        var page = ctx.Render<Bookings>();

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
        var page = ctx.Render<Bookings>();

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

    // Feature F4: joining the waitlist when a class is full is possible from the UI.
    [TestMethod]
    public void Bookings_JoinWaitlist_WhenClassFull_AddsToWaitlist()
    {
        var (ctx, store) = CreateContext();
        using var _ctx = ctx;

        // Fill the seeded Spin class (capacity 2)
        var spin = store.FindClass("C2")!;
        spin.ReserveSlot();
        spin.ReserveSlot();

        var page = ctx.Render<Bookings>();

        page.FindAll("select")[0].Change("M001");
        page.FindAll("select")[1].Change("C2");
        page.Find("button.btn-outline-warning").Click();

        Assert.AreEqual(1, store.Waitlist.Count);
        Assert.AreEqual("M001", store.Waitlist[0].Member.MemberId);
        Assert.IsTrue(page.Markup.Contains("waitlist for"));
    }

    // Feature F4: cancelling a booking auto-promotes the oldest waitlister.
    [TestMethod]
    public void Bookings_CancellingBooking_AutoPromotesOldestWaitlister()
    {
        var (ctx, store) = CreateContext();
        using var _ctx = ctx;

        // Two members, Spin class capacity 2 - fill it with Aroha, and have a second member waiting.
        var aroha = store.Members[0];
        var bob = new Membership("M002", "Bob", DateTime.Today.AddMonths(3));
        store.Members.Add(bob);

        var spin = store.FindClass("C2")!;
        var bookingSvc = new BookingService();
        var booking = bookingSvc.BookClass(aroha, spin).Booking!;
        spin.ReserveSlot(); // simulate another booking from someone else so spin is full
        store.Bookings.Add(booking);
        store.Waitlist.Add(new WaitlistEntry(bob, spin));

        var page = ctx.Render<Bookings>();

        // Cancel Aroha's booking - Bob should be auto-promoted.
        page.Find("button.btn-outline-danger").Click();

        Assert.AreEqual(0, store.Waitlist.Count, "Bob should have been promoted off the waitlist");
        Assert.AreEqual(2, store.Bookings.Count, "A new booking should exist for Bob");
        Assert.IsTrue(store.Bookings[1].Member.MemberId == "M002");
    }
}
