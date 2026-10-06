using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GymManagement;

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
    }
}