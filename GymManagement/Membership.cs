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
            MemberId = memberId;
            MemberName = memberName;
            ExpiryDate = expiryDate;
        }
    }
}