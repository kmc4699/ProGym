namespace GymManagement
{
    // Represents a member waiting for a spot on a class that's currently full.
    // Entries are ordered by the time they were added - oldest gets promoted first
    // when a cancellation frees up a slot.
    public class WaitlistEntry
    {
        public Membership Member { get; }
        public FitnessClass FitnessClass { get; }
        public DateTime AddedAt { get; }

        public WaitlistEntry(Membership member, FitnessClass fitnessClass)
        {
            Member = member ?? throw new ArgumentNullException(nameof(member));
            FitnessClass = fitnessClass ?? throw new ArgumentNullException(nameof(fitnessClass));
            AddedAt = DateTime.Now;
        }
    }
}
