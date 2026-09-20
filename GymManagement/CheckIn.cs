using System;

namespace GymManagement
{
    public class CheckIn
    {
        public string MemberId { get; }        // ID of the member checking in
        public DateTime CheckInTime { get; }   // Time the member checked in

        // Constructor used when checking in a member using their membership.
        // This is kept so the existing Members page can still use it.
        public CheckIn(Membership membership)
        {
            if (membership == null)
                throw new ArgumentNullException(nameof(membership));

            if (!membership.IsActive())
                throw new InvalidOperationException("Cannot check in: membership has expired.");

            MemberId = membership.MemberId;
            CheckInTime = DateTime.Now;
        }

        // FR13: Checks that the member has a valid booking before allowing
        // them to check in. This can be used from the Bookings page.
        public CheckIn(Booking booking)
        {
            if (booking == null)
                throw new ArgumentNullException(nameof(booking));

            if (booking.IsCancelled)
                throw new InvalidOperationException("Cannot check in: this booking has been cancelled.");

            if (!booking.Member.IsActive())
                throw new InvalidOperationException("Cannot check in: membership has expired.");

            MemberId = booking.Member.MemberId;
            CheckInTime = DateTime.Now;
        }
    }
}