# ProGym

A small **gym management system** built for the ENSE707 Software Quality Assurance
mid-project. Small gyms often manage class bookings and memberships manually
(paper sign-up sheets, spreadsheets), which causes overbooking, double-bookings,
and bookings on expired memberships with no clear feedback. ProGym is a
quality-focused prototype that addresses those issues with validation, reliable
rules, and clear success/failure messages.

## Modules

- **Membership** — register a member, active/expired status, renewal, days-until-expiry.
- **Class Booking** — book an active member into a class, reject full classes and
  expired members, cancel and release the slot back.
- **Check-in / Attendance** — record a member checking in (blocked if expired).
- **Reporting dashboard** — active vs expired members, class utilisation, total check-ins.

## Tech stack

- **C# / .NET 10**
- **Blazor Server** for the web UI (`GymManagement.Web`)
- **MSTest 4.3.3** for unit tests
- **In-memory storage** — no database, deliberate scope choice for the prototype

## Project layout

```
GymManagement.slnx                  solution
GymManagement/                      domain class library (all the rules live here)
GymManagement.Tests/                MSTest project — 30 tests
GymManagement.Web/                  Blazor Server web app
  Components/Pages/                 Home, Members, Classes, Bookings, Error, NotFound
  Services/GymDataStore.cs          shared in-memory data
  Program.cs                        DI wiring
```

## How to run

### In Visual Studio (recommended)
1. Open `GymManagement.slnx`.
2. Right-click **GymManagement.Web** → **Set as Startup Project**.
3. Pick the **https** launch profile, then **Ctrl+F5** (Start Without Debugging).
4. The browser opens at `https://localhost:7079/`.

### From the command line
```bash
dotnet run --project GymManagement.Web
```
Then browse to the URL printed in the console.

### Running the tests
```bash
dotnet test GymManagement.slnx
```
Expected: **30 passed, 0 failed.**

## What's built and what's not

**Working end-to-end:**
- Member registration + renewal + expiry status
- Class creation with capacity validation
- Booking with capacity + expired-member checks
- Cancellation that releases the slot back
- Dashboard with live summary numbers
- 30 unit tests covering all core rules

**Known gaps** (also documented in the mid-project report):
- **FR7** — reject past-date bookings (not yet enforced in `BookingService`)
- **FR8** — prevent double-booking / clashes (a member can currently book the
  same class multiple times; documented by a passing test)
- **FR13** — link check-in to a confirmed booking (currently only checks
  the membership is active)

## Team

| Member | Role |
| --- | --- |
| Ali Keshtkaran (22187440) | Booking + Reporting/Dashboard |
| Srikar Kurani (23203952) | Membership + Check-in |

Both team members commit directly to `master` (per the group GitHub rules) and
each averages at least two meaningful commits per week.

## More detail

- Mid-project report (Word document): submitted separately.
- Full requirements list (FR1–FR16, NFR1–NFR8) and traceability matrix live in
  the report's Task 2 and Task 6 sections.
