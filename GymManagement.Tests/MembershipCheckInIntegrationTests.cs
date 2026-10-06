using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GymManagement;

namespace GymManagement.Tests
{
    // Membership and check-in share the same expiry rule.
    // These tests ensure that both use the same rule.
    [TestClass]
    public class MembershipCheckInIntegrationTests
    {
        // An expired member is blocked from checking in, but can check in
        // again once the membership has been renewed.
        [TestMethod]
        public void CheckIn_AfterRenewingExpiredMembership_Succeeds()
        {
            var clock = new FakeClock { Today = DateTime.Today };
            var membership = new Membership("M1", "Jane Doe", DateTime.Today.AddDays(5), clock);
            clock.Today = DateTime.Today.AddDays(10);

            Assert.ThrowsExactly<InvalidOperationException>(() => new CheckIn(membership));

            membership.Renew(DateTime.Today.AddDays(40));
            var checkIn = new CheckIn(membership);

            Assert.AreEqual("M1", checkIn.MemberId);
        }
    }
}