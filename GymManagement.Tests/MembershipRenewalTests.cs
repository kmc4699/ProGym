using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GymManagement;

namespace GymManagement.Tests
{
    [TestClass]
    public class MembershipRenewalTests
    {
        // Renewing an expired membership makes it active again.
        [TestMethod]
        public void Membership_RenewAfterExpiry_BecomesActiveAgain()
        {
            var clock = new FakeClock { Today = DateTime.Today };
            var membership = new Membership("M1", "Jane Doe", DateTime.Today.AddDays(5), clock);
            clock.Today = DateTime.Today.AddDays(10);
            Assert.IsFalse(membership.IsActive());

            membership.Renew(DateTime.Today.AddDays(40));

            Assert.IsTrue(membership.IsActive());
        }

        // The smallest valid renewal is one day after the current expiry date.
        [TestMethod]
        public void Membership_RenewByOneDay_Succeeds()
        {
            var membership = new Membership("M1", "Jane Doe", DateTime.Today.AddDays(10));
            var newExpiry = membership.ExpiryDate.AddDays(1);

            membership.Renew(newExpiry);

            Assert.AreEqual(newExpiry, membership.ExpiryDate);
        }
    }
}