using System.Text.Json;
using GymManagement;

namespace GymManagement.Web.Services;

// Saves and loads the GymDataStore as a JSON file so the data
// is still available after the application is restarted.
// DTOs are used because the domain classes have get-only properties
// and validation in their constructors.
//
// Known limitation: memberships with an expiry date in the past
// cannot be loaded because the Membership constructor rejects them.
// These memberships are skipped and a warning is logged.
public class PersistenceService
{
    private readonly string _filePath;
    private readonly ILogger<PersistenceService>? _logger;

    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        WriteIndented = true
    };

    public PersistenceService(string filePath, ILogger<PersistenceService>? logger = null)
    {
        _filePath = filePath;
        _logger = logger;
    }

    public string FilePath => _filePath;

    // Saves the current data in the store to the JSON file.
    public void Save(GymDataStore store)
    {
        var dto = ToDto(store);
        var json = JsonSerializer.Serialize(dto, JsonOpts);

        var dir = Path.GetDirectoryName(_filePath);
        if (!string.IsNullOrWhiteSpace(dir) && !Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        File.WriteAllText(_filePath, json);
        _logger?.LogInformation("Persisted store to {Path}", _filePath);
    }

    // Loads saved data into the store.
    // Returns true if a save file was found and loaded successfully.
    // If no file exists, the existing seeded data is kept.
    public bool LoadInto(GymDataStore store)
    {
        if (!File.Exists(_filePath))
        {
            _logger?.LogInformation("No save file at {Path}, starting fresh", _filePath);
            return false;
        }

        var json = File.ReadAllText(_filePath);
        var dto = JsonSerializer.Deserialize<StoreDto>(json, JsonOpts);
        if (dto == null)
        {
            _logger?.LogWarning("Save file at {Path} was empty or invalid", _filePath);
            return false;
        }

        store.Members.Clear();
        store.Classes.Clear();
        store.Bookings.Clear();
        store.CheckIns.Clear();

        // Skip members whose expiry date has already passed.
        var today = DateTime.Today;
        foreach (var m in dto.Members ?? new())
        {
            if (m.ExpiryDate < today)
            {
                _logger?.LogInformation(
                    "Skipping expired member {Id} (expiry {Expiry:d})", m.MemberId, m.ExpiryDate);
                continue;
            }
            store.Members.Add(new Membership(m.MemberId, m.MemberName, m.ExpiryDate));
        }

        // Recreate the classes and restore their booked count.
        // ReserveSlot is called for each existing booking.
        var classesById = new Dictionary<string, FitnessClass>();
        foreach (var c in dto.Classes ?? new())
        {
            var fitnessClass = new FitnessClass(c.Id, c.Name, c.StartTime, c.Capacity);
            for (int i = 0; i < c.BookedCount && fitnessClass.HasAvailableSlot(); i++)
                fitnessClass.ReserveSlot();
            store.Classes.Add(fitnessClass);
            classesById[c.Id] = fitnessClass;
        }

        // Only restore bookings where the member and class still exist.
        var membersById = store.Members.ToDictionary(m => m.MemberId);
        foreach (var b in dto.Bookings ?? new())
        {
            if (!membersById.TryGetValue(b.MemberId, out var member)) continue;
            if (!classesById.TryGetValue(b.ClassId, out var fitnessClass)) continue;

            var booking = new Booking(member, fitnessClass);
            if (b.IsCancelled)
                booking.Cancel();
            store.Bookings.Add(booking);
        }

        // Restore check-ins and keep their original ClassId.
        foreach (var ci in dto.CheckIns ?? new())
        {
            if (!membersById.TryGetValue(ci.MemberId, out var member)) continue;
            store.CheckIns.Add(CheckIn.Restore(ci.MemberId, ci.ClassId, ci.CheckInTime, ci.Status));
        }

        _logger?.LogInformation("Loaded store from {Path}", _filePath);
        return true;
    }

    private static StoreDto ToDto(GymDataStore store) => new()
    {
        Members = store.Members
            .Select(m => new MemberDto(m.MemberId, m.MemberName, m.ExpiryDate))
            .ToList(),
        Classes = store.Classes
            .Select(c => new ClassDto(c.Id, c.Name, c.StartTime, c.Capacity, c.BookedCount))
            .ToList(),
        Bookings = store.Bookings
            .Select(b => new BookingDto(b.Member.MemberId, b.FitnessClass.Id, b.IsCancelled))
            .ToList(),
        CheckIns = store.CheckIns
            .Select(c => new CheckInDto(c.MemberId, c.ClassId, c.CheckInTime, c.Status))
            .ToList(),
    };

    // Internal DTO shapes
    private class StoreDto
    {
        public List<MemberDto> Members { get; set; } = new();
        public List<ClassDto> Classes { get; set; } = new();
        public List<BookingDto> Bookings { get; set; } = new();
        public List<CheckInDto> CheckIns { get; set; } = new();
    }

    private record MemberDto(string MemberId, string MemberName, DateTime ExpiryDate);
    private record ClassDto(string Id, string Name, DateTime StartTime, int Capacity, int BookedCount);
    private record BookingDto(string MemberId, string ClassId, bool IsCancelled);
    private record CheckInDto(string MemberId, string? ClassId, DateTime CheckInTime, AttendanceStatus Status);
}

