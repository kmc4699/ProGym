namespace GymManagement
{
    // The outcome of a waitlist operation (join or promote).
    public class WaitlistResult
    {
        public bool Success { get; }
        public string Message { get; }
        public WaitlistEntry? Entry { get; }
        public Booking? PromotedBooking { get; }

        public WaitlistResult(bool success, string message,
            WaitlistEntry? entry = null, Booking? promotedBooking = null)
        {
            Success = success;
            Message = message;
            Entry = entry;
            PromotedBooking = promotedBooking;
        }
    }

    // Manages the waitlist for full classes: joining when a class is full and
    // auto-promoting the oldest entry when a slot opens up again.
    public class WaitlistService
    {
        private readonly BookingService _bookingService;

        public WaitlistService(BookingService bookingService)
        {
            _bookingService = bookingService ?? throw new ArgumentNullException(nameof(bookingService));
        }

        // Adds a member to a class's waitlist. Rejects:
        //  - a null member or class
        //  - a class that isn't actually full (should just book it directly)
        //  - a member who's already on the waitlist for this class
        //  - a member whose membership has expired
        //  - a class whose start time has already passed
        public WaitlistResult Join(Membership member, FitnessClass fitnessClass,
            IReadOnlyList<WaitlistEntry> existingWaitlist)
        {
            if (member == null)
                return new WaitlistResult(false, "Waitlist join failed: member details are required.");
            if (fitnessClass == null)
                return new WaitlistResult(false, "Waitlist join failed: class details are required.");
            if (existingWaitlist == null)
                throw new ArgumentNullException(nameof(existingWaitlist));

            if (!member.IsActive())
                return new WaitlistResult(false,
                    $"Waitlist join failed: {member.MemberName}'s membership has expired.");

            if (fitnessClass.StartTime < DateTime.Now)
                return new WaitlistResult(false,
                    $"Waitlist join failed: '{fitnessClass.Name}' has already started.");

            if (fitnessClass.HasAvailableSlot())
                return new WaitlistResult(false,
                    $"Waitlist join failed: '{fitnessClass.Name}' still has free slots - book directly.");

            // One entry per member per class.
            foreach (var existing in existingWaitlist)
            {
                if (existing.Member.MemberId == member.MemberId
                    && existing.FitnessClass.Id == fitnessClass.Id)
                {
                    return new WaitlistResult(false,
                        $"Waitlist join failed: {member.MemberName} is already on the waitlist for '{fitnessClass.Name}'.");
                }
            }

            var entry = new WaitlistEntry(member, fitnessClass);
            return new WaitlistResult(true,
                $"{member.MemberName} is on the waitlist for '{fitnessClass.Name}'.", entry);
        }

        // Call this after a cancellation to try to promote the oldest waitlister
        // for the freed-up class to a real booking. If the booking service refuses
        // (eg. the member's membership expired while they were waiting), the entry
        // is still removed and the next one isn't tried - the caller can decide
        // whether to call PromoteNext again.
        public WaitlistResult PromoteNext(FitnessClass fitnessClass, List<WaitlistEntry> waitlist)
        {
            if (fitnessClass == null)
                throw new ArgumentNullException(nameof(fitnessClass));
            if (waitlist == null)
                throw new ArgumentNullException(nameof(waitlist));

            // Oldest entry for this class gets promoted first.
            var next = waitlist
                .Where(w => w.FitnessClass.Id == fitnessClass.Id)
                .OrderBy(w => w.AddedAt)
                .FirstOrDefault();

            if (next == null)
                return new WaitlistResult(false, "No waitlisters for this class.");

            // Remove from waitlist whether or not the booking succeeds - we don't want
            // to keep retrying the same entry on every cancellation.
            waitlist.Remove(next);

            var bookingResult = _bookingService.BookClass(next.Member, next.FitnessClass);
            if (!bookingResult.Success)
                return new WaitlistResult(false,
                    $"Could not promote {next.Member.MemberName} from waitlist: {bookingResult.Message}",
                    next);

            return new WaitlistResult(true,
                $"{next.Member.MemberName} was promoted from the waitlist and booked into '{fitnessClass.Name}'.",
                next, bookingResult.Booking);
        }
    }
}
