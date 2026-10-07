# ProGym - Quality Metrics

Snapshot of the quality metrics as of the last successful test run. This doc
feeds directly into the report's release-decision section (Task 8).

Version: 2026-10-07 (updated after FR8 closed and all final-phase features landed)
Reproduce: `dotnet test GymManagement.slnx --collect:"XPlat Code Coverage"`

## Test outcomes

| Metric                        | Value | Notes                                                    |
|-------------------------------|-------|----------------------------------------------------------|
| Total automated tests         | **90**| MSTest + bUnit + integration + performance                |
| Passing                       | **90**| 100% pass rate                                           |
| Failing                       | 0     |                                                          |
| Skipped                       | 0     |                                                          |
| Average run time (local)      | ~1 s  | Fast enough to run before every commit                    |
| CI matrix                     | Linux + Windows | Both runners have to pass before a PR can merge   |

### Test breakdown by type

| Type                    | Approx count | Example files                                           |
|-------------------------|--------------|---------------------------------------------------------|
| Unit (domain)           | ~55          | `BookingServiceTests`, `MembershipTests`, `CheckInTests`, `FitnessClassTests`, `WaitlistServiceTests` |
| Component (bUnit pages) | ~15          | `HomePageTests`, `BookingsPageTests`, `ClassesPageTests`, `MembersPageTests` |
| Integration             | ~5           | `PersistenceServiceTests`, `MembershipCheckInIntegrationTests` |
| Performance             | 2            | `PerformanceTests` (NFR3 - sub-second domain operations)  |
| Boundary                | ~8           | `MembershipBoundaryTests`, FR7 past-start-time tests      |
| CheckIn restore         | ~5           | `CheckInRestoreTests`                                     |

## Code coverage

Measured with `coverlet.collector` and reported in Cobertura format on every CI run.

| Metric                | Value    |
|-----------------------|----------|
| Line coverage         | **77.9%** (574 / 737 lines) |
| Branch coverage       | **73.6%** |

### Coverage by area

| Area                                          | Approx line coverage | Notes                                                                            |
|-----------------------------------------------|----------------------|----------------------------------------------------------------------------------|
| `GymManagement` (domain library)              | **~92%**             | All business rules covered; only defensive null checks are the misses            |
| `GymManagement.Web.Services.PersistenceService` | **~85%**           | Save/load round-trip tested end-to-end                                            |
| `GymManagement.Web.Components.Pages.Home`      | ~70%                 | bUnit tests cover heading, counters, utilisation table, attendance badges         |
| `GymManagement.Web.Components.Pages.Bookings`  | ~65%                 | bUnit tests cover book, cancel, waitlist join, waitlist auto-promote, validation  |
| `GymManagement.Web.Components.Pages.Classes`   | ~70%                 | bUnit tests cover seeded rendering, add, filter, hide-past checkbox               |
| `GymManagement.Web.Components.Pages.Members`   | ~65%                 | bUnit tests cover empty state, register, duplicate-ID, expiry warning badges      |
| `GymManagement.Web.Program` and layout        | 0%                   | Not testable through unit or component tests; smoke tests on real app cover this  |

The 78% figure is a strong number for a Blazor Server app - the domain lands around 92%, every user-facing page is bUnit-tested, and the remaining uncovered lines are mostly `Program.cs` and layout wiring that only run end-to-end.

## Defects

From `docs/DefectRegister.md`:

| Metric                                    | Value |
|-------------------------------------------|-------|
| Total defects logged                      | **3** |
| Severity: High                            | 2     |
| Severity: Medium                          | 1     |
| Severity: Low                             | 0     |
| Priority: P1 (fix immediately)            | 3     |
| Status: Closed (fix verified by a test)   | **3** |
| Status: Open                              | 0     |
| Median time-to-fix                        | Same day |

Every closed defect has an automated regression test that would catch a recurrence.

## Requirements coverage

From `docs/RequirementsTraceabilityMatrix.md`:

| Metric                                  | Value |
|-----------------------------------------|-------|
| Functional requirements                 | 23    |
| FRs implemented                         | **23** |
| FRs with automated tests                | 23    |
| FRs with a documented gap               | 0     |
| Non-functional requirements             | 8     |
| NFRs verified                           | 8     |
| Requirements traced to at least one test | **100%** (31 of 31) |

## Contribution activity

Snapshot from the shared repo:

| Metric                            | Value            |
|-----------------------------------|------------------|
| Total commits on `master`         | 92+              |
| Commits by Ali (`kmc4699`)        | 51               |
| Commits by Srikar (`Srikar Kurani`)| 41              |
| Weeks with >= 2 commits per member| Every week since project start |
| Longest gap between commits (any member) | 4 days       |

## Release decision

Verdict candidates and how each maps to the numbers above:

- **Ready for release** - all 23 FRs and 8 NFRs implemented and tested, 0 open defects, 90 tests passing, 78% line coverage with the domain layer at 92%, CI green on master, every change recorded in the shared repository with per-member commit history evenly split.
- **Conditionally ready** - would need at least one open requirement gap or a known defect that affected daily use, neither of which is present.
- **Not ready** - would need an open High-severity defect or an unimplemented core requirement, neither of which is present.

**Recommended verdict:** *Ready for release* for the prototype's intended scope. The remaining uncovered lines are mostly non-business framework wiring, and the gaps we had earlier in the final phase (FR8, duplicate check-ins, data loss on crash) have all been closed with regression tests that would catch a recurrence.

Future improvements (listed in Task 10 of the final report) include richer reporting, class-level cancellation, authentication and roles, and swapping the JSON persistence for a database. None of these are blockers for this prototype.

## Progress through the final phase

| Week             | Line coverage | Tests | FRs implemented |
|------------------|---------------|-------|-----------------|
| 2026-09-22       | 73.6%         | 48    | 17 / 18         |
| 2026-09-29       | 77.9%         | 67-72 | 17 / 18 (FR8 gap) |
| 2026-10-05       | 77.9%         | 81    | 17 / 18 (FR8 gap) |
| 2026-10-07       | **77.9%**     | **90**| **23 / 23** (all closed) |
