using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GymManagement;

namespace GymManagement.Tests
{
    [TestClass]
    public class MembershipTests
    {
        // Membership should be active if it hasn't expired yet
        [TestMethod]
        public void Membership_ActiveWhenExpiryInFuture_ReturnsTrue()
        {
            var membership = new Membership("M1", "Jane Doe", DateTime.Now.AddDays(30));
            Assert.IsTrue(membership.IsActive());
        }

        // Membership should be inactive once the expiry date has passed
        [TestMethod]
        public void Membership_ExpiredWhenExpiryInPast_ReturnsFalse()
        {
            var membership = new Membership("M1", "Jane Doe", DateTime.Now.AddDays(-1));
            Assert.IsFalse(membership.IsActive());
        }

        // Cannot create a membership without a member ID
        [TestMethod]
        public void Membership_EmptyMemberId_ThrowsException()
        {
            Assert.ThrowsException<ArgumentException>(() =>
                new Membership("", "Jane Doe", DateTime.Now.AddDays(30)));
        }
    }
}