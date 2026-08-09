using System;

namespace GymManagement
{
    public class CheckIn
    {
        public string MemberId { get; }        // who checked in
        public DateTime CheckInTime { get; }   // when they checked in

        public CheckIn(Membership membership)
        {
            if (membership == null)
                throw new ArgumentNullException(nameof(membership));

            // don't allow check-in if membership expired
            if (!membership.IsActive())
                throw new InvalidOperationException("Cannot check in: membership has expired.");

            MemberId = membership.MemberId;
            CheckInTime = DateTime.Now;
        }
    }
}