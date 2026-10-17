# ProGym - Quality Metrics

Snapshot of the quality metrics as of the last successful test run. This doc
feeds directly into the report's release-decision section (Task 8).

Version: 2026-10-17 (updated after DEF-04 fix and waitlist persistence tests)
Reproduce: `dotnet test GymManagement.slnx --collect:"XPlat Code Coverage"`

## Test outcomes

- Total automated tests: 92
- Passing: 92 (100% pass rate)
- Failing: 0
- Skipped: 0
- Average run time (local): about 3 seconds
- CI: runs on every push to master, on both Linux and Windows

### Test breakdown by type

- Unit (domain): 55 - BookingServiceTests (12), MembershipTests (10), CheckInTests (10), ReportingServiceTests (10), WaitlistServiceTests (6), FitnessClassTests (5), MembershipRenewalTests (2)
- Component (bUnit pages): 22 - BookingsPageTests (7), ClassesPageTests (6), MembersPageTests (5), HomePageTests (4)
- Integration: 7 - PersistenceServiceTests (4), WaitlistPersistenceTests (2), MembershipCheckInIntegrationTests (1)
- Performance: 2 - PerformanceTests (NFR3, sub-second domain operations)
- Boundary: 3 - MembershipBoundaryTests
- CheckIn restore: 3 - CheckInRestoreTests
- Total: 92

## Code coverage

Measured with `coverlet.collector` and reported in Cobertura format on every CI run.

- Line coverage: 78.7% (605 of 769 lines)
- Branch coverage: 74.3% (208 of 280 branches)

Approximate coverage by area (from the 2026-10-07 run):

- Domain library (GymManagement): about 92%. All business rules are covered; the only misses are defensive null checks.
- PersistenceService: about 85%. Save and load round trip is tested end to end.
- Home page: about 70%
- Bookings page: about 65%
- Classes page: about 70%
- Members page: about 65%
- Program.cs and layout: 0%. Not testable through unit or component tests.

The 79% overall figure is a strong number for a Blazor Server app. The domain lands around 92%, every user-facing page is bUnit-tested, and the remaining uncovered lines are mostly Program.cs and layout wiring that only run end to end.

## Defects

From `docs/DefectRegister.md`:

- Total defects logged: 4
- Severity High: 2, Medium: 2, Low: 0
- Priority P1: 4
- Closed (fix verified by a test): 4
- Open: 0
- Median time to fix: same day

Every closed defect has an automated regression test that would catch a recurrence.

## Requirements coverage

From `docs/RequirementsTraceabilityMatrix.md`:

- Functional requirements: 23, all implemented, all with automated tests, none with a documented gap
- Non-functional requirements: 8, all verified
- Requirements traced to at least one test: 100% (31 of 31)

## Contribution activity

Snapshot from the shared repo:

- Total commits on master (excluding merges): 96
- Commits by Ali (kmc4699): 53
- Commits by Srikar (Srikar Kurani): 43
- Both members made at least 2 commits per week since the project started
- Longest gap between commits (any member): 4 days

## Release decision

- Ready for release: all 23 FRs and 8 NFRs implemented and tested, 0 open defects, 92 tests passing, 79% line coverage with the domain layer at 92%, CI green on master, every change recorded in the shared repository.
- Conditionally ready: would need at least one open requirement gap or a known defect that affected daily use. Neither is present.
- Not ready: would need an open High-severity defect or an unimplemented core requirement. Neither is present.

Recommended verdict: Ready for release for the prototype's intended scope. The remaining uncovered lines are mostly non-business framework wiring, and the gaps we had earlier in the final phase (FR8, duplicate check-ins, data loss on crash, waitlist not persisted) have all been closed with regression tests that would catch a recurrence.

Future improvements (listed in Task 10 of the final report) include richer reporting, class-level cancellation, authentication and roles, and swapping the JSON persistence for a database. None of these are blockers for this prototype.

## Progress through the final phase

- 2026-09-22: 73.6% line coverage, 48 tests, 17 of 18 FRs
- 2026-09-29: 77.9% line coverage, 67-72 tests, 17 of 18 FRs (FR8 gap)
- 2026-10-05: 77.9% line coverage, 81 tests, 17 of 18 FRs (FR8 gap)
- 2026-10-07: 77.9% line coverage, 90 tests, 23 of 23 FRs (all closed)
- 2026-10-17: 78.7% line coverage, 92 tests, 23 of 23 FRs (DEF-04 fixed)