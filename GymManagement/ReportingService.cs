namespace GymManagement
{
    // A summary of how many memberships are active versus expired (FR14).
    public record MembershipSummary(int ActiveCount, int ExpiredCount)
    {
        public int TotalCount => ActiveCount + ExpiredCount;
    }

    // How full a single class is: booked out of capacity (FR15).
    public record ClassUtilisation(string ClassName, int Booked, int Capacity)
    {
        public int AvailableSlots => Capacity - Booked;
    }

    // Provides summary information for the reporting dashboard.
    // It only reads the current in-memory data (members, classes, check-ins)
    // through their existing public members, and produces counts to display.
    public class ReportingService
    {
        // FR14: how many memberships are active versus expired.
        public MembershipSummary GetMembershipSummary(IEnumerable<Membership> members)
        {
            if (members == null)
                throw new ArgumentNullException(nameof(members));

            var list = members.ToList();
            int active = list.Count(m => m.IsActive());

            return new MembershipSummary(active, list.Count - active);
        }

        // FR15: how full each class is (booked out of its capacity).
        public IReadOnlyList<ClassUtilisation> GetClassUtilisation(IEnumerable<FitnessClass> classes)
        {
            if (classes == null)
                throw new ArgumentNullException(nameof(classes));

            return classes
                .Select(c => new ClassUtilisation(c.Name, c.BookedCount, c.Capacity))
                .ToList();
        }

        // FR16: how many check-ins (attendances) have been recorded.
        public int GetTotalCheckIns(IEnumerable<CheckIn> checkIns)
        {
            if (checkIns == null)
                throw new ArgumentNullException(nameof(checkIns));

            return checkIns.Count();
        }
    }
}
