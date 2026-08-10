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
    }
}
