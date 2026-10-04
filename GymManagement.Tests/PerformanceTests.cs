using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GymManagement;

namespace GymManagement.Tests
{
    // NFR3: single-user actions should complete in under one second.
    // These only measure the domain logic (no UI, no disk), so they work as a
    // quick regression guard rather than a full performance test.
    [TestClass]
    public class PerformanceTests
    {
        private const int Operations = 10_000;
        private const long LimitMs = 1000;

        // Creating and querying lots of memberships should stay well under the limit.
        [TestMethod]
        public void Membership_ManyOperations_CompletesUnderOneSecond()
        {
            var timer = Stopwatch.StartNew();

            for (int i = 0; i < Operations; i++)
            {
                var membership = new Membership($"M{i}", "Jane Doe", DateTime.Today.AddDays(30));
                membership.IsActive();
                membership.DaysUntilExpiry();
            }

            timer.Stop();
            Assert.IsTrue(timer.ElapsedMilliseconds < LimitMs,
                $"Membership operations took {timer.ElapsedMilliseconds} ms");
        }

        // Same idea for check-ins.
        [TestMethod]
        public void CheckIn_ManyCheckIns_CompletesUnderOneSecond()
        {
            var membership = new Membership("M1", "Jane Doe", DateTime.Today.AddDays(30));
            var timer = Stopwatch.StartNew();

            for (int i = 0; i < Operations; i++)
                _ = new CheckIn(membership);

            timer.Stop();
            Assert.IsTrue(timer.ElapsedMilliseconds < LimitMs,
                $"Check-in operations took {timer.ElapsedMilliseconds} ms");
        }
    }
}