using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GymManagement;

namespace GymManagement.Tests
{
    [TestClass]
    public class MembershipBoundaryTests
    {
        // Boundary: on the expiry date itself, the membership is still active.
        [TestMethod]
        public void Membership_OnExpiryDay_IsActive()
        {
            var clock = new FakeClock { Today = DateTime.Today };
            var membership = new Membership("M1", "Jane Doe", DateTime.Today.AddDays(5), clock);

            clock.Today = DateTime.Today.AddDays(5);

            Assert.IsTrue(membership.IsActive());
        }

        // Boundary: the day after the expiry date, the membership is no longer active.
        [TestMethod]
        public void Membership_DayAfterExpiry_IsNotActive()
        {
            var clock = new FakeClock { Today = DateTime.Today };
            var membership = new Membership("M1", "Jane Doe", DateTime.Today.AddDays(5), clock);

            clock.Today = DateTime.Today.AddDays(6);

            Assert.IsFalse(membership.IsActive());
        }

        // A name made up only of spaces should be rejected, same as an empty one.
        [TestMethod]
        public void Membership_WhitespaceName_ThrowsException()
        {
            Assert.ThrowsExactly<ArgumentException>(() =>
                new Membership("M1", "   ", DateTime.Today.AddDays(30)));
        }
    }
}