using Microsoft.VisualStudio.TestTools.UnitTesting;
using GymManagement;

namespace GymManagement.Tests
{
    [TestClass]
    public class BookingServiceTests
    {
        private static Membership ActiveMember() =>
            new Membership("M001", "Aroha Smith", DateTime.Today.AddMonths(1));

        private static Membership ExpiredMember() =>
            new Membership("M002", "John Chen", DateTime.Today.AddDays(-1));

        private static FitnessClass NewClass(int capacity) =>
            new FitnessClass("C001", "Yoga", DateTime.Today.AddDays(1), capacity);

        [TestMethod]
        public void BookClass_ActiveMemberAndAvailableClass_ReturnsSuccess()
        {
            var service = new BookingService();

            var result = service.BookClass(ActiveMember(), NewClass(5));

            Assert.IsTrue(result.Success);
            Assert.IsNotNull(result.Booking);
        }

        [TestMethod]
        public void BookClass_Success_ReservesASlot()
        {
            var service = new BookingService();
            var yoga = NewClass(2);

            service.BookClass(ActiveMember(), yoga);

            Assert.AreEqual(1, yoga.BookedCount);
            Assert.AreEqual(1, yoga.AvailableSlots);
        }

        [TestMethod]
        public void BookClass_ExpiredMembership_ReturnsFailure()
        {
            var service = new BookingService();
            var yoga = NewClass(5);

            var result = service.BookClass(ExpiredMember(), yoga);

            Assert.IsFalse(result.Success);
            Assert.IsTrue(result.Message.Contains("expired"));
            Assert.AreEqual(0, yoga.BookedCount); // no slot taken on failure
        }

        [TestMethod]
        public void BookClass_FullClass_ReturnsFailure()
        {
            var service = new BookingService();
            var yoga = NewClass(1);
            service.BookClass(ActiveMember(), yoga); // fills the only slot

            var result = service.BookClass(ActiveMember(), yoga);

            Assert.IsFalse(result.Success);
            Assert.IsTrue(result.Message.Contains("fully booked"));
        }

        [TestMethod]
        public void CancelBooking_ExistingBooking_ReleasesSlot()
        {
            var service = new BookingService();
            var yoga = NewClass(2);
            var booking = service.BookClass(ActiveMember(), yoga).Booking!;

            var result = service.CancelBooking(booking);

            Assert.IsTrue(result.Success);
            Assert.IsTrue(booking.IsCancelled);
            Assert.AreEqual(0, yoga.BookedCount); // slot released back
        }

        [TestMethod]
        public void CancelBooking_AlreadyCancelled_ReturnsFailure()
        {
            var service = new BookingService();
            var yoga = NewClass(2);
            var booking = service.BookClass(ActiveMember(), yoga).Booking!;
            service.CancelBooking(booking);

            var result = service.CancelBooking(booking);

            Assert.IsFalse(result.Success);
        }
    }
}
