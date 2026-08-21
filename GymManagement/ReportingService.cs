namespace GymManagement
{
    // A summary of how many memberships are active versus expired (FR14).
    public record MembershipSummary(int ActiveCount, int ExpiredCount)
    {
        public int TotalCount => ActiveCount + ExpiredCount;
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
    }
}
