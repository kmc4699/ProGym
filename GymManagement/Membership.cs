using System;

namespace GymManagement
{
    public class Membership
    {
        private readonly IClock _clock;

        public string MemberId { get; }       // unique ID for the member
        public string MemberName { get; }     // member's name
        public DateTime ExpiryDate { get; private set; }  // when membership ends

        public Membership(string memberId, string memberName, DateTime expiryDate)
            : this(memberId, memberName, expiryDate, new SystemClock())
        {
        }

        // Lets tests pass in a fake clock instead of the real one
        public Membership(string memberId, string memberName, DateTime expiryDate, IClock clock)
        {
            _clock = clock ?? throw new ArgumentNullException(nameof(clock));

            if (string.IsNullOrWhiteSpace(memberId))
                throw new ArgumentException("Member ID is required.");

            if (string.IsNullOrWhiteSpace(memberName))
                throw new ArgumentException("Member name is required.");

            // Can't register a new membership that's already expired
            if (expiryDate < _clock.Today)
                throw new ArgumentException("Expiry date cannot be in the past.");

            MemberId = memberId;
            MemberName = memberName;
            ExpiryDate = expiryDate;
        }

        // Checks if membership hasn't expired yet
        public bool IsActive()
        {
            return _clock.Today <= ExpiryDate;
        }

        // Extends the membership by setting a new, later expiry date
        public void Renew(DateTime newExpiryDate)
        {
            if (newExpiryDate <= ExpiryDate)
                throw new ArgumentException("New expiry date must be after the current expiry date.");

            ExpiryDate = newExpiryDate;
        }

        // Returns how many days are left until the membership expires
        // (negative number if already expired)
        public int DaysUntilExpiry()
        {
            return (ExpiryDate.Date - _clock.Today).Days;
        }
    }
}