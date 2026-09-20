using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GymManagement;

namespace GymManagement.Tests
{
    [TestClass]
    public class CheckInTests
    {
        // An active membership should allow the member to check in.
        [TestMethod]
        public void CheckIn_ActiveMembership_Succeeds()
        {
            var membership = new Membership("M1", "Jane Doe", DateTime.Now.AddDays(30));
            var checkIn = new CheckIn(membership);
            Assert.AreEqual("M1", checkIn.MemberId);
        }

        // An expired membership should prevent the member from checking in.
        [TestMethod]
        public void CheckIn_ExpiredMembership_ThrowsException()
        {
            var clock = new FakeClock { Today = DateTime.Today };
            var membership = new Membership("M1", "Jane Doe", DateTime.Today.AddDays(5), clock);
            clock.Today = DateTime.Today.AddDays(10);

            Assert.ThrowsExactly<InvalidOperationException>(() => new CheckIn(membership));
        }

        // A null membership should not allow a check-in.
        [TestMethod]
        public void CheckIn_NullMembership_ThrowsException()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => new CheckIn((Membership)null!));
        }

        // FR13: A valid booking should allow the member to check in.
        [TestMethod]
        public void CheckIn_FromValidBooking_Succeeds()
        {
            var membership = new Membership("M1", "Jane Doe", DateTime.Now.AddDays(30));
            var fitnessClass = new FitnessClass("C1", "Yoga", DateTime.Now.AddDays(1), 5);
            var booking = new Booking(membership, fitnessClass);

            var checkIn = new CheckIn(booking);

            Assert.AreEqual("M1", checkIn.MemberId);
        }

        // FR13: A cancelled booking should not allow the member to check in.
        [TestMethod]
        public void CheckIn_FromCancelledBooking_ThrowsException()
        {
            var membership = new Membership("M1", "Jane Doe", DateTime.Now.AddDays(30));
            var fitnessClass = new FitnessClass("C1", "Yoga", DateTime.Now.AddDays(1), 5);
            var booking = new Booking(membership, fitnessClass);
            booking.Cancel();

            Assert.ThrowsExactly<InvalidOperationException>(() => new CheckIn(booking));
        }

        // FR13: A null booking should not be allowed.
        [TestMethod]
        public void CheckIn_NullBooking_ThrowsException()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => new CheckIn((Booking)null!));
        }
    }
}