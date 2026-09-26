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
        public string? ClassId { get; }        // which class this check-in relates to, if known
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
            ClassId = null;
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
            ClassId = booking.FitnessClass.Id;
            CheckInTime = DateTime.Now;
            Status = status;
        }

        // Used to restore a CheckIn from saved data.
        // The normal validation is skipped because the CheckIn
        // was already validated when it was originally created.
        private CheckIn(string memberId, string? classId, DateTime checkInTime, AttendanceStatus status)
        {
            MemberId = memberId;
            ClassId = classId;
            CheckInTime = checkInTime;
            Status = status;
        }

        // Creates a CheckIn from previously saved data.
        // This is used when loading CheckIns from storage instead
        // of creating a new check-in through the normal constructors.
        public static CheckIn Restore(string memberId, string? classId, DateTime checkInTime, AttendanceStatus status)
        {
            if (string.IsNullOrWhiteSpace(memberId))
                throw new ArgumentException("Member ID is required.");

            return new CheckIn(memberId, classId, checkInTime, status);
        }
    }
}