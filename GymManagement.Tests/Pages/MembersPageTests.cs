using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GymManagement;
using GymManagement.Web.Components.Pages;
using GymManagement.Web.Services;

namespace GymManagement.Tests.Pages;

// Component tests for the Members page. Drive the register form and the
// renew form the same way a user would, and check the DOM afterwards.
[TestClass]
public class MembersPageTests
{
    private static (Bunit.BunitContext ctx, GymDataStore store) CreateContext()
    {
        var store = new GymDataStore();
        var tempFile = Path.Combine(Path.GetTempPath(), $"progym-members-{Guid.NewGuid():N}.json");

        var ctx = new Bunit.BunitContext();
        ctx.Services.AddSingleton(store);
        ctx.Services.AddSingleton(new PersistenceService(tempFile));
        return (ctx, store);
    }

    [TestMethod]
    public void Members_InitiallyShowsEmptyState()
    {
        var (ctx, _) = CreateContext();
        using var _ctx = ctx;

        var page = ctx.Render<Members>();

        Assert.IsTrue(page.Markup.Contains("No members registered"));
    }

    [TestMethod]
    public void Members_RegisterForm_AddsMemberAndShowsInTable()
    {
        var (ctx, store) = CreateContext();
        using var _ctx = ctx;
        var page = ctx.Render<Members>();

        // Fill the register form - always re-find the inputs after each change.
        page.FindAll("input")[0].Change("M001");                        // Member ID
        page.FindAll("input")[1].Change("Aroha Smith");                 // Member Name
        page.FindAll("input[type=date]")[0]
            .Change(DateTime.Today.AddMonths(6).ToString("yyyy-MM-dd"));

        page.FindAll("form")[0].Submit();

        Assert.AreEqual(1, store.Members.Count);
        Assert.IsTrue(page.Markup.Contains("Aroha Smith"));
        Assert.IsTrue(page.Find("div.alert-success").TextContent.Contains("Registered"));
    }

    [TestMethod]
    public void Members_RegisterForm_DuplicateId_ShowsErrorAndDoesNotAdd()
    {
        var (ctx, store) = CreateContext();
        using var _ctx = ctx;

        // Pre-seed a member with the same ID we're about to type
        store.Members.Add(new Membership("M001", "Existing", DateTime.Today.AddMonths(3)));
        var page = ctx.Render<Members>();

        page.FindAll("input")[0].Change("M001");
        page.FindAll("input")[1].Change("Duplicate Attempt");
        page.FindAll("form")[0].Submit();

        Assert.AreEqual(1, store.Members.Count);
        Assert.IsTrue(page.Find("div.alert-danger").TextContent.Contains("already registered"));
    }

    // Feature F2: expiry warning badges based on DaysUntilExpiry.
    [TestMethod]
    public void Members_ExpiringSoonMember_ShowsYellowExpiringSoonBadge()
    {
        var (ctx, store) = CreateContext();
        using var _ctx = ctx;

        // 5 days until expiry -> below the 14-day warning threshold
        store.Members.Add(new Membership("M1", "Soon Expiring", DateTime.Today.AddDays(5)));
        var page = ctx.Render<Members>();

        var row = page.FindAll("table tbody tr").First(r => r.TextContent.Contains("Soon Expiring"));
        var badge = row.QuerySelector("span.bg-warning");
        Assert.IsNotNull(badge);
        Assert.IsTrue(badge!.TextContent.Contains("Expiring soon"));
    }

    [TestMethod]
    public void Members_ActiveMemberWithLongExpiry_ShowsGreenActiveBadge()
    {
        var (ctx, store) = CreateContext();
        using var _ctx = ctx;

        // 6 months out -> well above the warning threshold
        store.Members.Add(new Membership("M1", "Safe Member", DateTime.Today.AddMonths(6)));
        var page = ctx.Render<Members>();

        var row = page.FindAll("table tbody tr").First(r => r.TextContent.Contains("Safe Member"));
        var greenBadge = row.QuerySelector("span.bg-success");
        Assert.IsNotNull(greenBadge);
        Assert.AreEqual("Active", greenBadge!.TextContent.Trim());
    }
}
