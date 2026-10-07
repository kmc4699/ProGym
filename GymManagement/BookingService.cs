namespace GymManagement
{
    // Handles booking a member into a fitness class.
    // It enforces the booking rules: the member's membership must be active,
    // and the class must have a free slot. Every attempt returns a clear result.
    public class BookingService
    {
        public BookingResult BookClass(Membership member, FitnessClass fitnessClass)
        {
            if (member == null)
                return new BookingResult(false, "Booking failed: member details are required.");
            if (fitnessClass == null)
                return new BookingResult(false, "Booking failed: class details are required.");

            // FR7: reject bookings for a class whose start time has already passed.
            if (fitnessClass.StartTime < DateTime.Now)
                return new BookingResult(false,
                    $"Booking failed: '{fitnessClass.Name}' has already started or ended.");

            // FR6: the member's membership must be active.
            if (!member.IsActive())
                return new BookingResult(false,
                    $"Booking failed: {member.MemberName}'s membership has expired.");

            // FR5: the class must have a free slot.
            if (!fitnessClass.HasAvailableSlot())
                return new BookingResult(false,
                    $"Booking failed: '{fitnessClass.Name}' is fully booked.");

            // FR9 + FR4: reserve a slot and create the booking.
            fitnessClass.ReserveSlot();
            var booking = new Booking(member, fitnessClass);

            // FR11: clear success message.
            return new BookingResult(true,
                $"Booking confirmed: {member.MemberName} is booked into '{fitnessClass.Name}'.",
                booking);
        }

        // FR8: same as BookClass above, but also rejects a duplicate booking -
        // the same member cannot have two active bookings for the same class.
        // Cancelled bookings don't count as duplicates, so a member can re-book
        // the same class after they cancelled.
        public BookingResult BookClass(Membership member, FitnessClass fitnessClass,
            IEnumerable<Booking> existingBookings)
        {
            if (existingBookings == null)
                throw new ArgumentNullException(nameof(existingBookings));

            if (member != null && fitnessClass != null)
            {
                foreach (var existing in existingBookings)
                {
                    if (!existing.IsCancelled
                        && existing.Member.MemberId == member.MemberId
                        && existing.FitnessClass.Id == fitnessClass.Id)
                    {
                        return new BookingResult(false,
                            $"Booking failed: {member.MemberName} is already booked into '{fitnessClass.Name}'.");
                    }
                }
            }

            return BookClass(member!, fitnessClass!);
        }

        // FR10: cancels an existing booking and releases the class slot back
        // so it can be booked again. Returns a clear result either way.
        public BookingResult CancelBooking(Booking booking)
        {
            if (booking == null)
                return new BookingResult(false, "Cancellation failed: booking details are required.");
            if (booking.IsCancelled)
                return new BookingResult(false, "Cancellation failed: this booking is already cancelled.");

            booking.Cancel();
            booking.FitnessClass.ReleaseSlot();

            return new BookingResult(true,
                $"Booking cancelled for '{booking.FitnessClass.Name}'. The slot is now free.",
                booking);
        }
    }
}
