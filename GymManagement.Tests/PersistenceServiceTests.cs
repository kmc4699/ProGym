using Microsoft.VisualStudio.TestTools.UnitTesting;
using GymManagement;
using GymManagement.Web.Services;

namespace GymManagement.Tests;

// Real file-system tests: each test uses a unique temp file and cleans up
// after itself so the tests can run in any order (or in parallel) without
// stepping on each other.
[TestClass]
public class PersistenceServiceTests
{
    private string _tempFile = string.Empty;

    [TestInitialize]
    public void Init()
    {
        _tempFile = Path.Combine(Path.GetTempPath(),
            $"progym-test-{Guid.NewGuid():N}.json");
    }

    [TestCleanup]
    public void Cleanup()
    {
        if (File.Exists(_tempFile))
            File.Delete(_tempFile);
    }

    [TestMethod]
    public void Save_ThenLoad_RestoresMembersAndClasses()
    {
        // Arrange - a store with one added member on top of the seeded classes
        var original = new GymDataStore();
        original.Members.Add(new Membership("M100", "Aroha Smith", DateTime.Today.AddMonths(6)));
        var persistence = new PersistenceService(_tempFile);

        // Act - save, then load into a fresh store
        persistence.Save(original);
        var restored = new GymDataStore();
        restored.Members.Clear();     // clear the freshly seeded state so we can see what was loaded
        restored.Classes.Clear();
        bool loaded = persistence.LoadInto(restored);

        // Assert
        Assert.IsTrue(loaded);
        Assert.AreEqual(1, restored.Members.Count);
        Assert.AreEqual("Aroha Smith", restored.Members[0].MemberName);
        Assert.AreEqual(2, restored.Classes.Count); // the seeded Yoga + Spin
    }

    [TestMethod]
    public void Save_ThenLoad_RestoresBookedCountOnClasses()
    {
        var original = new GymDataStore();
        var member = new Membership("M1", "Test", DateTime.Today.AddMonths(1));
        original.Members.Add(member);
        // Reserve 2 slots on the seeded Yoga class (capacity 10)
        var yoga = original.FindClass("C1")!;
        yoga.ReserveSlot();
        yoga.ReserveSlot();
        var persistence = new PersistenceService(_tempFile);

        persistence.Save(original);
        var restored = new GymDataStore();
        restored.Members.Clear(); restored.Classes.Clear();
        persistence.LoadInto(restored);

        var restoredYoga = restored.FindClass("C1")!;
        Assert.AreEqual(2, restoredYoga.BookedCount);
        Assert.AreEqual(8, restoredYoga.AvailableSlots);
    }

    [TestMethod]
    public void Save_ThenLoad_RestoresBookingsAndCancelledState()
    {
        var original = new GymDataStore();
        var member = new Membership("M1", "Test", DateTime.Today.AddMonths(1));
        original.Members.Add(member);
        var yoga = original.FindClass("C1")!;

        var b1 = new Booking(member, yoga);           // active
        var b2 = new Booking(member, yoga);
        b2.Cancel();                                   // cancelled
        original.Bookings.Add(b1);
        original.Bookings.Add(b2);
        var persistence = new PersistenceService(_tempFile);

        persistence.Save(original);
        var restored = new GymDataStore();
        restored.Members.Clear(); restored.Classes.Clear();
        persistence.LoadInto(restored);

        Assert.AreEqual(2, restored.Bookings.Count);
        Assert.IsFalse(restored.Bookings[0].IsCancelled);
        Assert.IsTrue(restored.Bookings[1].IsCancelled);
    }

    [TestMethod]
    public void LoadInto_NoSaveFile_ReturnsFalseAndLeavesStoreUntouched()
    {
        var store = new GymDataStore();
        int classesBefore = store.Classes.Count;
        var persistence = new PersistenceService(_tempFile);

        bool loaded = persistence.LoadInto(store);

        Assert.IsFalse(loaded);
        Assert.AreEqual(classesBefore, store.Classes.Count); // seeded classes still there
    }
}
