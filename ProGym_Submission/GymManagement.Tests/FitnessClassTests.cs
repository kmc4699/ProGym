using Microsoft.VisualStudio.TestTools.UnitTesting;
using GymManagement;

namespace GymManagement.Tests
{
    [TestClass]
    public class FitnessClassTests
    {
        [TestMethod]
        public void Constructor_ValidDetails_StartsEmptyWithFullCapacity()
        {
            var yoga = new FitnessClass("C001", "Yoga", DateTime.Today.AddDays(1), 10);

            Assert.AreEqual(10, yoga.Capacity);
            Assert.AreEqual(0, yoga.BookedCount);
            Assert.AreEqual(10, yoga.AvailableSlots);
            Assert.IsTrue(yoga.HasAvailableSlot());
        }

        [TestMethod]
        public void Constructor_ZeroCapacity_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() =>
                new FitnessClass("C001", "Yoga", DateTime.Today.AddDays(1), 0));
        }

        [TestMethod]
        public void ReserveSlot_WhenSpaceAvailable_IncreasesBookedCount()
        {
            var yoga = new FitnessClass("C001", "Yoga", DateTime.Today.AddDays(1), 2);

            yoga.ReserveSlot();

            Assert.AreEqual(1, yoga.BookedCount);
            Assert.AreEqual(1, yoga.AvailableSlots);
        }

        [TestMethod]
        public void ReserveSlot_WhenFull_ThrowsException()
        {
            var yoga = new FitnessClass("C001", "Yoga", DateTime.Today.AddDays(1), 1);
            yoga.ReserveSlot(); // fills the only slot

            Assert.Throws<InvalidOperationException>(() => yoga.ReserveSlot());
        }

        [TestMethod]
        public void ReleaseSlot_AfterReserving_FreesTheSlot()
        {
            var yoga = new FitnessClass("C001", "Yoga", DateTime.Today.AddDays(1), 2);
            yoga.ReserveSlot();

            yoga.ReleaseSlot();

            Assert.AreEqual(0, yoga.BookedCount);
        }
    }
}
