using System;

namespace GymManagement
{
    // FR12: Stores whether the member attended or was marked as a no-show.
    // Attended is the default since it is the normal check-in status.
    public enum AttendanceStatus
    {
        Attended,
        NoShow
    }

    public class CheckIn
    {
        public string MemberId { get; }        // ID of the member checking in
        public DateTime CheckInTime { get; }   // Time the member checked in
        public AttendanceStatus Status { get; }

        // Checks in a member using their membership.
        // The existing Members page can still use it.
        public CheckIn(Membership membership, AttendanceStatus status = AttendanceStatus.Attended)
        {
            if (membership == null)
                throw new ArgumentNullException(nameof(membership));

            if (!membership.IsActive())
                throw new InvalidOperationException("Cannot check in: membership has expired.");

            MemberId = membership.MemberId;
            CheckInTime = DateTime.Now;
            Status = status;
        }

        // FR13: Checks that the member has a valid and active booking
        // before allowing them to check in.
        public CheckIn(Booking booking, AttendanceStatus status = AttendanceStatus.Attended)
        {
            if (booking == null)
                throw new ArgumentNullException(nameof(booking));

            if (booking.IsCancelled)
                throw new InvalidOperationException("Cannot check in: this booking has been cancelled.");

            if (!booking.Member.IsActive())
                throw new InvalidOperationException("Cannot check in: membership has expired.");

            MemberId = booking.Member.MemberId;
            CheckInTime = DateTime.Now;
            Status = status;
        }
    }
}