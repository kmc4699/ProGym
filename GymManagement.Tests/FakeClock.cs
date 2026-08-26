namespace GymManagement.Tests
{
    // A controllable clock for tests (set to today, will change later)
    // In the same test to simulate time passing
    public class FakeClock : IClock
    {
        public DateTime Today { get; set; } = DateTime.Now.Date;
    }
}