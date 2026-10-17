using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GymManagement;
using GymManagement.Web.Services;

namespace GymManagement.Tests
{
    // CheckIn.Restore rebuilds a saved check-in.
    // These tests check that it keeps the saved values instead of creating new ones.
    [TestClass]
    public class CheckInRestoreTests
    {
        // The original check-in time is kept and not reset to the current time
        [TestMethod]
        public void CheckIn_Restore_KeepsOriginalCheckInTime()
        {
            var savedTime = new DateTime(2026, 9, 1, 9, 30, 0);

            var restored = CheckIn.Restore("M1", "C1", savedTime, AttendanceStatus.Attended);

            Assert.AreEqual(savedTime, restored.CheckInTime);
        }

        // The class ID must come back the same as it was saved (DEF-01).
        [TestMethod]
        public void CheckIn_Restore_PreservesClassId()
        {
            var restored = CheckIn.Restore("M1", "C1", DateTime.Now, AttendanceStatus.Attended);

            Assert.AreEqual("C1", restored.ClassId);
        }

        // Since Check-ins made from a membership have no class, null must be allowed.
        [TestMethod]
        public void CheckIn_Restore_AllowsNullClassId()
        {
            var restored = CheckIn.Restore("M1", null, DateTime.Now, AttendanceStatus.Attended);

            Assert.IsNull(restored.ClassId);
        }

        // A blank member ID should still be rejected when restoring.
        [TestMethod]
        public void CheckIn_Restore_BlankMemberId_ThrowsException()
        {
            Assert.ThrowsExactly<ArgumentException>(() =>
                CheckIn.Restore("   ", "C1", DateTime.Now, AttendanceStatus.Attended));
        }

        // Full round trip for DEF-01: save a check-in made from a booking, load it
        // into a fresh store, and check the class ID is still there.
        [TestMethod]
        public void Save_ThenLoad_KeepsCheckInClassId()
        {
            var tempFile = Path.Combine(Path.GetTempPath(), $"progym-ci-{Guid.NewGuid():N}.json");
            try
            {
                var original = new GymDataStore();
                var member = new Membership("M1", "Ana", DateTime.Today.AddMonths(1));
                original.Members.Add(member);
                var yoga = original.FindClass("C1")!;
                var booking = new Booking(member, yoga);
                original.Bookings.Add(booking);
                original.CheckIns.Add(new CheckIn(booking));
                var persistence = new PersistenceService(tempFile);

                persistence.Save(original);
                var restored = new GymDataStore();
                persistence.LoadInto(restored);

                Assert.AreEqual(1, restored.CheckIns.Count);
                Assert.AreEqual("C1", restored.CheckIns[0].ClassId);
            }
            finally
            {
                if (File.Exists(tempFile)) File.Delete(tempFile);
            }
        }
    }
}