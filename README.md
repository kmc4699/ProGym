# ProGym

[![CI](https://github.com/kmc4699/ProGym/actions/workflows/ci.yml/badge.svg)](https://github.com/kmc4699/ProGym/actions/workflows/ci.yml)

A gym management system built for the ENSE707 Software Quality Assurance group
project (Assessment 1 + Assessment 2, Semester 2 2026). Small gyms often manage
memberships and class bookings manually (paper sign-in sheets, spreadsheets),
which leads to overbooking classes, letting expired memberships check in, or
bookings failing with no clear feedback. ProGym fixes that with proper
validation, consistent rules, clear success/failure messages, and automated
tests behind every rule.

## What it does

- **Membership** - register a member, track active/expired status, renew, see days until expiry (with yellow/red badges for soon-to-expire or expired members).
- **Check-in** - record a member attending a class, tied to a confirmed booking, with Attended / No-Show status.
- **Class booking** - book an active member into a class, reject full classes, expired members, past-start classes, and duplicate bookings; cancel a booking and free up (or auto-promote from the waitlist) the slot.
- **Waitlist** - join a waitlist when a class is full; oldest waitlister is auto-promoted when someone cancels.
- **Dashboard** - active vs expired member counts, class utilisation, total check-ins, and per-class attendance breakdown.
- **CSV export** - download the full bookings list from the Bookings page.
- **Persistence** - everything survives an app restart; data is saved after every change to a JSON file.

## Tech stack

- C# / .NET 10
- Blazor Server for the web UI (`GymManagement.Web`)
- Domain logic lives in a separate class library (`GymManagement`) with no UI dependencies
- MSTest + bUnit for tests, coverlet for coverage, GitHub Actions for CI (Linux + Windows matrix)
- JSON file persistence (no database - deliberate scope decision for a prototype)

## Project layout

```
GymManagement/            Domain class library (business rules, no UI)
GymManagement.Tests/      MSTest + bUnit test projects (94 tests)
GymManagement.Web/        Blazor Server UI, DI composition, JSON persistence
docs/                     Live quality artefacts (RTM, DefectRegister, QualityMetrics, TestCases)
.github/workflows/ci.yml  CI pipeline (build, test, coverage, matrix on 2 OSes)
```

## Running it

### In Visual Studio

1. Open `GymManagement.slnx`.
2. Right-click **GymManagement.Web** -> Set as a Startup Project.
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

As of the final submission: **94 tests pass, 0 fail**, in about 3 seconds locally.

## Final status (as of 2026-10-17)

- 23 of 23 functional requirements implemented and tested
- 8 of 8 non-functional requirements verified
- 94 automated tests (unit, bUnit component, integration, performance, boundary) - all passing
- 79.5% line coverage (611 / 769), 75.0% branch coverage; domain layer around 92%
- 4 defects logged across the project, all closed with regression tests behind each one
- CI green on master on both Linux and Windows runners

## Live project artefacts

The `docs/` folder is updated continuously, not just at submission:

- [`docs/RequirementsTraceabilityMatrix.md`](docs/RequirementsTraceabilityMatrix.md) - every FR/NFR mapped to code, tests, and any related defects
- [`docs/TestCases.md`](docs/TestCases.md) - formal test case catalogue (ID, inputs, expected result, status)
- [`docs/DefectRegister.md`](docs/DefectRegister.md) - every defect with severity, priority, root cause, fix, prevention test, and lifecycle
- [`docs/QualityMetrics.md`](docs/QualityMetrics.md) - current test counts, coverage, defect summary, contribution activity, and the release decision

## Team

- Ali Keshtkaran (22187440) - Booking service, persistence, CI pipeline, bUnit page tests, final-phase feature UI (attendance breakdown, expiry badges, class filter, waitlist page, CSV export)
- Srikar Kurani (23203952) - AttendanceStatus and booking-based CheckIn, Membership.DaysUntilExpiry and renewal, WaitlistService, performance tests, FakeClock pattern, waitlist persistence

Both members commit directly to `master` per the lecturer's rule; the shared
history is the individual contribution evidence.

## Reports

- Mid-project report (A1): submitted 2026-08-31
- Final project report + presentation (A2): submitted 2026-10-18
