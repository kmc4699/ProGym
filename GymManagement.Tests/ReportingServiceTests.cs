using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GymManagement;

namespace GymManagement.Tests
{
    [TestClass]
    public class ReportingServiceTests
    {
        private static Membership Active(string id) =>
            new Membership(id, "Member " + id, DateTime.Today.AddMonths(1));

        // was expired via a past date directly - now uses a fake clock instead
        private static Membership Expired(string id)
        {
            var clock = new FakeClock { Today = DateTime.Today };
            var membership = new Membership(id, "Member " + id, DateTime.Today.AddDays(5), clock);
            clock.Today = DateTime.Today.AddDays(10);
            return membership;
        }

        [TestMethod]
        public void GetMembershipSummary_CountsActiveAndExpired()
        {
            var reporting = new ReportingService();
            var members = new[] { Active("M1"), Active("M2"), Expired("M3") };

            var summary = reporting.GetMembershipSummary(members);

            Assert.AreEqual(2, summary.ActiveCount);
            Assert.AreEqual(1, summary.ExpiredCount);
            Assert.AreEqual(3, summary.TotalCount);
        }

        [TestMethod]
        public void GetClassUtilisation_ReturnsBookedAndCapacityPerClass()
        {
            var reporting = new ReportingService();
            var yoga = new FitnessClass("C1", "Yoga", DateTime.Today.AddDays(1), 5);
            yoga.ReserveSlot();
            yoga.ReserveSlot();

            var result = reporting.GetClassUtilisation(new[] { yoga });

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Yoga", result[0].ClassName);
            Assert.AreEqual(2, result[0].Booked);
            Assert.AreEqual(5, result[0].Capacity);
            Assert.AreEqual(3, result[0].AvailableSlots);
        }

        [TestMethod]
        public void GetTotalCheckIns_CountsAllCheckIns()
        {
            var reporting = new ReportingService();
            var member = Active("M1");
            var checkIns = new[] { new CheckIn(member), new CheckIn(member) };

            int total = reporting.GetTotalCheckIns(checkIns);

            Assert.AreEqual(2, total);
        }

        [TestMethod]
        public void GetMembershipSummary_NullInput_ThrowsException()
        {
            var reporting = new ReportingService();
            Assert.Throws<ArgumentNullException>(() => reporting.GetMembershipSummary(null!));
        }

        [TestMethod]
        public void GetClassUtilisation_NullInput_ThrowsException()
        {
            var reporting = new ReportingService();
            Assert.Throws<ArgumentNullException>(() => reporting.GetClassUtilisation(null!));
        }

        [TestMethod]
        public void GetTotalCheckIns_NullInput_ThrowsException()
        {
            var reporting = new ReportingService();
            Assert.Throws<ArgumentNullException>(() => reporting.GetTotalCheckIns(null!));
        }
    
        // Attendance breakdown: counts Attended vs NoShow per class using ClassId.
        [TestMethod]
        public void GetAttendanceByClass_MixedStatuses_CountsCorrectlyPerClass()
        {
            var reporting = new ReportingService();
            var member = Active("M1");
            var yoga = new FitnessClass("C1", "Yoga", DateTime.Today.AddDays(1), 10);
            var spin = new FitnessClass("C2", "Spin", DateTime.Today.AddDays(1), 5);

            var yogaBooking = new Booking(member, yoga);
            var spinBooking = new Booking(member, spin);

            var checkIns = new List<CheckIn>
            {
                new CheckIn(yogaBooking, AttendanceStatus.Attended),
                new CheckIn(yogaBooking, AttendanceStatus.NoShow),
                new CheckIn(spinBooking, AttendanceStatus.Attended)
            };

            var result = reporting.GetAttendanceByClass(new[] { yoga, spin }, checkIns);

            Assert.AreEqual(2, result.Count);
            var yogaRow = result.First(a => a.ClassId == "C1");
            Assert.AreEqual(1, yogaRow.AttendedCount);
            Assert.AreEqual(1, yogaRow.NoShowCount);
            Assert.AreEqual(2, yogaRow.TotalCheckIns);
            var spinRow = result.First(a => a.ClassId == "C2");
            Assert.AreEqual(1, spinRow.AttendedCount);
            Assert.AreEqual(0, spinRow.NoShowCount);
        }

        // Classes with no check-ins should still appear in the result with zero counts.
        [TestMethod]
        public void GetAttendanceByClass_NoCheckIns_ReturnsZerosPerClass()
        {
            var reporting = new ReportingService();
            var yoga = new FitnessClass("C1", "Yoga", DateTime.Today.AddDays(1), 10);

            var result = reporting.GetAttendanceByClass(new[] { yoga }, new List<CheckIn>());

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(0, result[0].AttendedCount);
            Assert.AreEqual(0, result[0].NoShowCount);
        }

        [TestMethod]
        public void GetAttendanceByClass_NullClasses_ThrowsException()
        {
            var reporting = new ReportingService();
            Assert.Throws<ArgumentNullException>(
                () => reporting.GetAttendanceByClass(null!, new List<CheckIn>()));
        }

        [TestMethod]
        public void GetAttendanceByClass_NullCheckIns_ThrowsException()
        {
            var reporting = new ReportingService();
            Assert.Throws<ArgumentNullException>(
                () => reporting.GetAttendanceByClass(new List<FitnessClass>(), null!));
        }
}
}