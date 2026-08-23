using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GymManagement;

namespace GymManagement.Tests
{
    [TestClass]
    public class MembershipTests
    {
        [TestMethod]
        public void Membership_ActiveWhenExpiryInFuture_ReturnsTrue()
        {
            var membership = new Membership("M1", "Jane Doe", DateTime.Now.AddDays(30));
            Assert.IsTrue(membership.IsActive());
        }

        [TestMethod]
        public void Membership_ExpiredWhenExpiryInPast_ReturnsFalse()
        {
            var membership = new Membership("M1", "Jane Doe", DateTime.Now.AddDays(-1));
            Assert.IsFalse(membership.IsActive());
        }

        [TestMethod]
        public void Membership_EmptyMemberId_ThrowsException()
        {
            Assert.ThrowsExactly<ArgumentException>(() =>
                new Membership("", "Jane Doe", DateTime.Now.AddDays(30)));
        }

        [TestMethod]
        public void Membership_Renew_ExtendsExpiryDate()
        {
            var membership = new Membership("M1", "Jane Doe", DateTime.Now.AddDays(10));
            var newExpiry = DateTime.Now.AddDays(40);
            membership.Renew(newExpiry);
            Assert.AreEqual(newExpiry, membership.ExpiryDate);
        }

        [TestMethod]
        public void Membership_Renew_EarlierDate_ThrowsException()
        {
            var membership = new Membership("M1", "Jane Doe", DateTime.Now.AddDays(30));
            Assert.ThrowsExactly<ArgumentException>(() =>
                membership.Renew(DateTime.Now.AddDays(5)));
        }

        [TestMethod]
        public void Membership_PastExpiryDate_ThrowsException()
        {
            Assert.ThrowsExactly<ArgumentException>(() =>
                new Membership("M1", "Jane Doe", DateTime.Now.Date.AddDays(-5)));
        }

        [TestMethod]
        public void Membership_DaysUntilExpiry_ReturnsCorrectCount()
        {
            var membership = new Membership("M1", "Jane Doe", DateTime.Now.Date.AddDays(10));
            Assert.AreEqual(10, membership.DaysUntilExpiry());
        }

        [TestMethod]
        public void Membership_ExpiryIsToday_IsStillActive()
        {
            var membership = new Membership("M1", "Jane Doe", DateTime.Now.Date);
            Assert.IsTrue(membership.IsActive());
        }
    }
}