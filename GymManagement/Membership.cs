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

            MemberId = memberId;
            MemberName = memberName;
            ExpiryDate = expiryDate;
        }

        // checks if membership hasn't expired yet
        public bool IsActive()
        {
            return DateTime.Now <= ExpiryDate;
        }
    }
}