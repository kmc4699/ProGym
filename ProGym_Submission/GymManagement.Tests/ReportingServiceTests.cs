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
    }
}