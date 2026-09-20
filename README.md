# ProGym

A gym management system built for our ENSE707 Software Quality Assurance
mid-project. Small gyms often manage memberships and class bookings manually
(paper sign-in sheets, spreadsheets), which leads to problems like
overbooking classes, letting expired memberships check in, or bookings
failing with no clear feedback. ProGym is our attempt at fixing that with
proper validation, consistent rules, and clear success/failure messages.

## What it does

- **Membership** - register a member, track active/expired status, renew, see days until expiry.
- **Check-in** - record a member checking in (blocked if their membership's expired).
- **Class booking** - book an active member into a class, reject full classes and expired members, cancel a booking and free up the slot.
- **Dashboard** - active vs expired members, how full each class is, total check-ins.

## Tech stack

- C# / .NET 10
- Blazor Server for the web UI (`GymManagement.Web`)
- MSTest for unit tests
- Everything's stored in memory for now, there is no database. That was deliberately done to keep the prototype simple for our mid project. We will expand it for the final report.

## Project layout


## Running it

### In Visual Studio

1. Open `GymManagement.slnx`.
2. Right-click **GymManagement.Web** → Set as a Startup Project.
3. Pick the **https** launch profile, hit Ctrl+F5.
4. The browser opens at `https://localhost:7079/`.

### From the command line

```bash
dotnet run --project GymManagement.Web
```

Then open whatever URL shows up in the console.

### Running the tests

```bash
dotnet test GymManagement.slnx
```

It should say "30 passed, 0 failed."

## What works and what doesn't yet

**Working:**
- Member registration, renewal, expiry status
- Class creation with capacity checks
- Booking with capacity + expired-member checks
- Cancelling a booking releases the slot
- Dashboard with live numbers
- 30 unit tests covering the core rules

**Still to do** (also in the report):
- Reject bookings for classes that have already started/passed
- Stop the same member booking the same class more than once — currently allowed, documented by a test
- Tie check-in to an actual confirmed booking, not just "is the membership active"

## Team

- Srikar Kurani (23203952) - Membership + Check-in
- Ali Keshtkaran (22187440) - Booking + Dashboard

We're both committing straight to `master`.

## More information

Full mid-project report (Word doc) will be submitted separately.

