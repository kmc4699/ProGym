using Microsoft.VisualStudio.TestTools.UnitTesting;
using GymManagement;

namespace GymManagement.Tests;

[TestClass]
public class WaitlistServiceTests
{
    private static Membership ActiveMember(string id = "M001", string name = "Aroha") =>
        new Membership(id, name, DateTime.Today.AddMonths(3));

    private static FitnessClass FullClass(int capacity = 2)
    {
        var fc = new FitnessClass("C1", "Yoga", DateTime.Today.AddDays(1), capacity);
        for (int i = 0; i < capacity; i++) fc.ReserveSlot();
        return fc;
    }

    [TestMethod]
    public void Join_FullClassAndValidMember_ReturnsSuccessAndEntry()
    {
        var waitlistSvc = new WaitlistService(new BookingService());
        var result = waitlistSvc.Join(ActiveMember(), FullClass(), new List<WaitlistEntry>());

        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Entry);
        Assert.AreEqual("M001", result.Entry!.Member.MemberId);
    }

    [TestMethod]
    public void Join_ClassStillHasSlots_ReturnsFailure()
    {
        var waitlistSvc = new WaitlistService(new BookingService());
        var hasRoom = new FitnessClass("C1", "Yoga", DateTime.Today.AddDays(1), 5);

        var result = waitlistSvc.Join(ActiveMember(), hasRoom, new List<WaitlistEntry>());

        Assert.IsFalse(result.Success);
        Assert.IsTrue(result.Message.Contains("still has free slots"));
    }

    [TestMethod]
    public void Join_SameMemberTwice_SecondAttemptFails()
    {
        var waitlistSvc = new WaitlistService(new BookingService());
        var fullClass = FullClass();
        var member = ActiveMember();
        var waitlist = new List<WaitlistEntry>();

        var first = waitlistSvc.Join(member, fullClass, waitlist);
        waitlist.Add(first.Entry!);
        var second = waitlistSvc.Join(member, fullClass, waitlist);

        Assert.IsTrue(first.Success);
        Assert.IsFalse(second.Success);
        Assert.IsTrue(second.Message.Contains("already on the waitlist"));
    }

    [TestMethod]
    public void Join_PastClass_ReturnsFailure()
    {
        var waitlistSvc = new WaitlistService(new BookingService());
        var pastClass = new FitnessClass("C1", "Yoga", DateTime.Now.AddHours(-1), 1);
        pastClass.ReserveSlot(); // make it full

        var result = waitlistSvc.Join(ActiveMember(), pastClass, new List<WaitlistEntry>());

        Assert.IsFalse(result.Success);
        Assert.IsTrue(result.Message.Contains("already started"));
    }

    [TestMethod]
    public void PromoteNext_OldestWaitlisterGetsBooked()
    {
        var bookingSvc = new BookingService();
        var waitlistSvc = new WaitlistService(bookingSvc);
        var fullClass = FullClass();

        var alice = ActiveMember("M001", "Alice");
        var bob = ActiveMember("M002", "Bob");

        var waitlist = new List<WaitlistEntry>
        {
            new WaitlistEntry(alice, fullClass)   // added first -> promoted first
        };
        Thread.Sleep(10); // ensure a strictly later AddedAt
        waitlist.Add(new WaitlistEntry(bob, fullClass));

        // Simulate a cancellation freeing one slot
        fullClass.ReleaseSlot();

        var result = waitlistSvc.PromoteNext(fullClass, waitlist);

        Assert.IsTrue(result.Success);
        Assert.AreEqual("Alice", result.Entry!.Member.MemberName);
        Assert.IsNotNull(result.PromotedBooking);
        Assert.AreEqual(1, waitlist.Count); // Alice removed, Bob still waiting
        Assert.AreEqual("Bob", waitlist[0].Member.MemberName);
    }

    [TestMethod]
    public void PromoteNext_EmptyWaitlist_ReturnsFailureSilently()
    {
        var waitlistSvc = new WaitlistService(new BookingService());
        var fullClass = FullClass();

        var result = waitlistSvc.PromoteNext(fullClass, new List<WaitlistEntry>());

        Assert.IsFalse(result.Success);
        Assert.IsTrue(result.Message.Contains("No waitlisters"));
    }
}
