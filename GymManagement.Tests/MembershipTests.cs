using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GymManagement;

namespace GymManagement.Tests
{
    [TestClass]
    public class MembershipTests
    {
        // membership should be active if it hasn't expired yet
        [TestMethod]
        public void Membership_ActiveWhenExpiryInFuture_ReturnsTrue()
        {
            var membership = new Membership("M1", "Jane Doe", DateTime.Now.AddDays(30));
            Assert.IsTrue(membership.IsActive());
        }

        // membership should be inactive once the expiry date has passed
        [TestMethod]
        public void Membership_ExpiredWhenExpiryInPast_ReturnsFalse()
        {
            var membership = new Membership("M1", "Jane Doe", DateTime.Now.AddDays(-1));
            Assert.IsFalse(membership.IsActive());
        }

        // can't create a membership without a member ID
        [TestMethod]
        public void Membership_EmptyMemberId_ThrowsException()
        {
            Assert.ThrowsExactly<ArgumentException>(() =>
                new Membership("", "Jane Doe", DateTime.Now.AddDays(30)));
        }

        // renewing should push the expiry date further into the future
        [TestMethod]
        public void Membership_Renew_ExtendsExpiryDate()
        {
            var membership = new Membership("M1", "Jane Doe", DateTime.Now.AddDays(10));
            var newExpiry = DateTime.Now.AddDays(40);

            membership.Renew(newExpiry);

            Assert.AreEqual(newExpiry, membership.ExpiryDate);
        }

        // Can't renew to an earlier date than the current expiry
        [TestMethod]
        public void Membership_Renew_EarlierDate_ThrowsException()
        {
            var membership = new Membership("M1", "Jane Doe", DateTime.Now.AddDays(30));

            Assert.ThrowsExactly<ArgumentException>(() =>
                membership.Renew(DateTime.Now.AddDays(5)));
        }
    }
}