namespace GymManagement
{
    // Lets us fake "today" in tests instead of relying on the real clock
    public interface IClock
    {
        DateTime Today { get; }
    }

    // The real clock, used everywhere except tests
    public class SystemClock : IClock
    {
        public DateTime Today => DateTime.Now.Date;
    }
}