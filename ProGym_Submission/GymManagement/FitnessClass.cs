namespace GymManagement
{
    // Represents one gym class/session members can book into, e.g. "Yoga at 9am".
    // It knows its capacity and how many bookings exist, and controls its own
    // slot count so the class can never be overbooked.
    public class FitnessClass
    {
        public string Id { get; }
        public string Name { get; }
        public DateTime StartTime { get; }
        public int Capacity { get; }
        public int BookedCount { get; private set; }

        public FitnessClass(string id, string name, DateTime startTime, int capacity)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Class ID is required.");
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Class name is required.");
            if (capacity <= 0)
                throw new ArgumentException("Capacity must be greater than zero.");

            Id = id;
            Name = name;
            StartTime = startTime;
            Capacity = capacity;
            BookedCount = 0;
        }

        // How many places are still free.
        public int AvailableSlots => Capacity - BookedCount;

        // True while there is at least one free place.
        public bool HasAvailableSlot() => BookedCount < Capacity;

        // Reserves one place. Throws if the class is already full —
        // this is the rule that stops a class ever being overbooked.
        public void ReserveSlot()
        {
            if (!HasAvailableSlot())
                throw new InvalidOperationException($"'{Name}' is fully booked.");

            BookedCount++;
        }

        // Frees one place, used when a booking is cancelled.
        public void ReleaseSlot()
        {
            if (BookedCount > 0)
                BookedCount--;
        }
    }
}
