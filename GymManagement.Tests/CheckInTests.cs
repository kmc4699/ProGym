using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GymManagement;

namespace GymManagement.Tests
{
    [TestClass]
    public class CheckInTests
    {
        // Should be able to check in with an active membership
        [TestMethod]
        public void CheckIn_ActiveMembership_Succeeds()
        {
            var membership = new Membership("M1", "Jane Doe", DateTime.Now.AddDays(30));
            var checkIn = new CheckIn(membership);
            Assert.AreEqual("M1", checkIn.MemberId);
        }

        // Expired membership should block the check-in
        [TestMethod]
        public void CheckIn_ExpiredMembership_ThrowsException()
        {
            var membership = new Membership("M1", "Jane Doe", DateTime.Now.AddDays(-1));
            Assert.ThrowsExactly<InvalidOperationException>(() => new CheckIn(membership));
        }

        // Check-in shouldn't work without a membership at all
        [TestMethod]
        public void CheckIn_NullMembership_ThrowsException()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => new CheckIn(null!));
        }
    }
}