using System;

namespace GymManagement
{
    public class CheckIn
    {
        public string MemberId { get; }
        public DateTime CheckInTime { get; }

        public CheckIn(Membership membership)
        {
            if (membership == null)
                throw new ArgumentNullException(nameof(membership));

            if (!membership.IsActive())
                throw new InvalidOperationException("Cannot check in: membership has expired.");

            MemberId = membership.MemberId;
            CheckInTime = DateTime.Now;
        }
    }
}