namespace GymManagement
{
    // The outcome of a booking or cancellation attempt: whether it succeeded,
    // a clear message explaining the result, and the booking itself when successful.
    // This is what lets the system always give clear feedback (FR11).
    public class BookingResult
    {
        public bool Success { get; }
        public string Message { get; }
        public Booking? Booking { get; }

        public BookingResult(bool success, string message, Booking? booking = null)
        {
            Success = success;
            Message = message;
            Booking = booking;
        }
    }
}
