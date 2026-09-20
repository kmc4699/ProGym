namespace GymManagement
{
    // A record of one booking: which member booked which class,
    // and whether the booking has been cancelled.
    public class Booking
    {
        public Membership Member { get; }
        public FitnessClass FitnessClass { get; }
        public bool IsCancelled { get; private set; }

        public Booking(Membership member, FitnessClass fitnessClass)
        {
            Member = member ?? throw new ArgumentNullException(nameof(member));
            FitnessClass = fitnessClass ?? throw new ArgumentNullException(nameof(fitnessClass));
            IsCancelled = false;
        }

        // Marks this booking as cancelled. A booking can only be cancelled once.
        public void Cancel()
        {
            if (IsCancelled)
                throw new InvalidOperationException("This booking has already been cancelled.");

            IsCancelled = true;
        }
    }
}
