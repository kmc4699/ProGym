using System;

namespace GymManagement
{
    public class Membership
    {
        public string MemberId { get; }       // unique ID for the member
        public string MemberName { get; }     // member's name
        public DateTime ExpiryDate { get; private set; }  // when membership ends

        public Membership(string memberId, string memberName, DateTime expiryDate)
        {
            if (string.IsNullOrWhiteSpace(memberId))
                throw new ArgumentException("Member ID is required.");

            if (string.IsNullOrWhiteSpace(memberName))
                throw new ArgumentException("Member name is required.");

            if (expiryDate < DateTime.Now.Date)
                throw new ArgumentException("Expiry date cannot be in the past.");

            MemberId = memberId;
            MemberName = memberName;
            ExpiryDate = expiryDate;
        }

        // checks if membership hasn't expired yet
        public bool IsActive()
        {
            return DateTime.Now <= ExpiryDate;
        }

        // extends the membership by setting a new, later expiry date
        public void Renew(DateTime newExpiryDate)
        {
            if (newExpiryDate <= ExpiryDate)
                throw new ArgumentException("New expiry date must be after the current expiry date.");

            ExpiryDate = newExpiryDate;
        }

        // returns how many days are left until the membership expires
        // (negative number if already expired)
        public int DaysUntilExpiry()
        {
            return (ExpiryDate.Date - DateTime.Now.Date).Days;
        }
    }
}