using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using GymManagement;



namespace GymManagement.Tests

{

    [TestClass]

    public class BookingServiceTests

    {

        private static Membership ActiveMember() =>

        new Membership("M001", "Aroha Smith", DateTime.Today.AddMonths(1));



        // was expired via a past date directly - now uses a fake clock instead  

        private static Membership ExpiredMember()

        {

            var clock = new FakeClock { Today = DateTime.Today };

            var membership = new Membership("M002", "John Chen", DateTime.Today.AddDays(5), clock);

            clock.Today = DateTime.Today.AddDays(10);

            return membership;

        }



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

            Assert.AreEqual(0, yoga.BookedCount);

        }



        [TestMethod]

        public void BookClass_FullClass_ReturnsFailure()

        {

            var service = new BookingService();

            var yoga = NewClass(1);

            service.BookClass(ActiveMember(), yoga);



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

            Assert.AreEqual(0, yoga.BookedCount);

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



        // The base overload (no bookings list) does not check for duplicates - FR8 is enforced by the overload below 

        [TestMethod]

        public void BookClass_BaseOverload_DoesNotCheckForDuplicates()

        {

            var service = new BookingService();

            var member = ActiveMember();

            var yoga = NewClass(5);



            var first = service.BookClass(member, yoga);

            var second = service.BookClass(member, yoga);



            Assert.IsTrue(first.Success);

            Assert.IsTrue(second.Success);  

            Assert.AreEqual(2, yoga.BookedCount);

        }

    

        // FR7: a class whose start time has already passed must not accept bookings.
        [TestMethod]
        public void BookClass_PastStartTime_ReturnsFailure()
        {
            var service = new BookingService();
            var pastClass = new FitnessClass("C002", "Morning Yoga", DateTime.Now.AddHours(-1), 5);

            var result = service.BookClass(ActiveMember(), pastClass);

            Assert.IsFalse(result.Success);
            Assert.IsTrue(result.Message.Contains("already started"));
            Assert.AreEqual(0, pastClass.BookedCount);
        }

        // FR7: a class scheduled for the future must still succeed as before.
        [TestMethod]
        public void BookClass_FutureStartTime_ReturnsSuccess()
        {
            var service = new BookingService();
            var futureClass = new FitnessClass("C003", "Evening Spin", DateTime.Now.AddHours(2), 5);

            var result = service.BookClass(ActiveMember(), futureClass);

            Assert.IsTrue(result.Success);
            Assert.AreEqual(1, futureClass.BookedCount);
        }

        // FR8: the overload that takes an existing-bookings list rejects a duplicate
        // booking - same member cannot have two active bookings for the same class.
        [TestMethod]
        public void BookClass_DuplicateWithExistingBookingsList_IsRejected()
        {
            var service = new BookingService();
            var yoga = NewClass(5);
            var member = ActiveMember();
            var bookings = new List<Booking>();

            var first = service.BookClass(member, yoga, bookings);
            bookings.Add(first.Booking!);
            var second = service.BookClass(member, yoga, bookings);

            Assert.IsTrue(first.Success);
            Assert.IsFalse(second.Success);
            Assert.IsTrue(second.Message.Contains("already booked"));
            Assert.AreEqual(1, yoga.BookedCount);
        }

        // FR8: a member can still book a *different* class even if they are
        // already booked into another class.
        [TestMethod]
        public void BookClass_SameMemberDifferentClass_WithList_Succeeds()
        {
            var service = new BookingService();
            var yoga = NewClass(5);
            var spin = new FitnessClass("C099", "Spin", DateTime.Today.AddDays(1), 5);
            var member = ActiveMember();
            var bookings = new List<Booking>();

            var first = service.BookClass(member, yoga, bookings);
            bookings.Add(first.Booking!);
            var second = service.BookClass(member, spin, bookings);

            Assert.IsTrue(first.Success);
            Assert.IsTrue(second.Success);
        }

        // FR8: a cancelled booking does not count as a duplicate, so a member
        // can re-book a class they previously cancelled.
        [TestMethod]
        public void BookClass_SameClassAfterCancellation_WithList_IsAllowed()
        {
            var service = new BookingService();
            var yoga = NewClass(5);
            var member = ActiveMember();
            var bookings = new List<Booking>();

            var first = service.BookClass(member, yoga, bookings);
            bookings.Add(first.Booking!);
            service.CancelBooking(first.Booking!);

            var second = service.BookClass(member, yoga, bookings);

            Assert.IsTrue(second.Success);
        }
}

}