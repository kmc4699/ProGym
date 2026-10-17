using Microsoft.VisualStudio.TestTools.UnitTesting;
using GymManagement;
using GymManagement.Web.Services;

namespace GymManagement.Tests;

// Checks the waitlist survives a save and load (DEF-04).
[TestClass]
public class WaitlistPersistenceTests
{
    private string _tempFile = string.Empty;

    [TestInitialize]
    public void Init()
    {
        _tempFile = Path.Combine(Path.GetTempPath(), $"progym-wl-{Guid.NewGuid():N}.json");
    }

    [TestCleanup]
    public void Cleanup()
    {
        if (File.Exists(_tempFile)) File.Delete(_tempFile);
    }

    [TestMethod]
    public void Save_ThenLoad_RestoresWaitlistOrderAndJoinTime()
    {
        var original = new GymDataStore();
        var ana = new Membership("M1", "Ana", DateTime.Today.AddMonths(1));
        var ben = new Membership("M2", "Ben", DateTime.Today.AddMonths(1));
        original.Members.Add(ana);
        original.Members.Add(ben);
        var spin = original.FindClass("C2")!;
        var firstJoin = new DateTime(2026, 10, 1, 9, 0, 0);
        original.Waitlist.Add(new WaitlistEntry(ana, spin, firstJoin));
        original.Waitlist.Add(new WaitlistEntry(ben, spin, firstJoin.AddMinutes(5)));
        var persistence = new PersistenceService(_tempFile);

        persistence.Save(original);
        var restored = new GymDataStore();
        persistence.LoadInto(restored);

        Assert.AreEqual(2, restored.Waitlist.Count);
        Assert.AreEqual("M1", restored.Waitlist[0].Member.MemberId);
        Assert.AreEqual(firstJoin, restored.Waitlist[0].AddedAt);
        Assert.AreEqual("M2", restored.Waitlist[1].Member.MemberId);
    }

    [TestMethod]
    public void Save_ThenLoad_EmptyWaitlist_StaysEmpty()
    {
        var original = new GymDataStore();
        var persistence = new PersistenceService(_tempFile);

        persistence.Save(original);
        var restored = new GymDataStore();
        persistence.LoadInto(restored);

        Assert.AreEqual(0, restored.Waitlist.Count);
    }
}