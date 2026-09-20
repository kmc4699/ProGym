using GymManagement;



namespace GymManagement.Web.Services

{

    // Shared in-memory data so every page sees the same members, classes, 

    // bookings, and check-ins. Registered as a Singleton in Program.cs so 

    // all sessions use the same instance  

    public class GymDataStore

    {

        public List<Membership> Members { get; } = new();

        public List<FitnessClass> Classes { get; } = new();

        public List<Booking> Bookings { get; } = new();

        public List<CheckIn> CheckIns { get; } = new();



        public GymDataStore()

        {

            SeedSampleClasses();

        }



        private void SeedSampleClasses()

        {

            Classes.Add(new FitnessClass("C1", "Yoga", DateTime.Today.AddDays(1).AddHours(9), 10));

            Classes.Add(new FitnessClass("C2", "Spin", DateTime.Today.AddDays(1).AddHours(18), 2));

        }



        public Membership? FindMember(string memberId) =>

            Members.Find(m => m.MemberId == memberId);



        public FitnessClass? FindClass(string classId) =>

            Classes.Find(c => c.Id == classId);

    }

}