using System;

namespace GymManagement
{
    public class Membership
    {
        public string MemberId { get; }
        public string MemberName { get; }
        public DateTime ExpiryDate { get; private set; }

        public Membership(string memberId, string memberName, DateTime expiryDate)
        {
            if (string.IsNullOrWhiteSpace(memberId))
                throw new ArgumentException("Member ID is required.");

            if (string.IsNullOrWhiteSpace(memberName))
                throw new ArgumentException("Member name is required.");

            MemberId = memberId;
            MemberName = memberName;
            ExpiryDate = expiryDate;
        }

        // A membership is active if today's date is on or before the expiry date.
        public bool IsActive()
        {
            return DateTime.Now <= ExpiryDate;
        }
    }
}