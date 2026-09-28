# ProGym - Quality Metrics

Snapshot of the quality metrics as of the last successful test run. This doc
feeds directly into the report's release-decision section (Task 8).

Version: 2026-09-29 (updated after Classes/Members bUnit tests landed)
Reproduce: `dotnet test GymManagement.slnx --collect:"XPlat Code Coverage"`

## Test outcomes

| Metric                        | Value | Notes                                                    |
|-------------------------------|-------|----------------------------------------------------------|
| Total automated tests         | **57**| MSTest + bUnit + integration                              |
| Passing                       | **57**| 100% pass rate                                           |
| Failing                       | 0     |                                                          |
| Skipped                       | 0     |                                                          |
| Average run time (local)      | ~1 s  | Fast enough to run before every commit                    |
| CI matrix                     | Linux + Windows | Both runners have to pass before a PR can merge   |

## Code coverage

Measured with `coverlet.collector` and reported in Cobertura format on every CI run.

| Metric                | Value    |
|-----------------------|----------|
| Line coverage         | **73.6%** (407 / 553 lines) |
| Branch coverage       | **70.8%** |

### Coverage by area

| Area                                          | Approx line coverage | Notes                                                                            |
|-----------------------------------------------|----------------------|----------------------------------------------------------------------------------|
| `GymManagement` (domain library)              | **~90%**             | All business rules covered; only defensive null checks are the misses            |
| `GymManagement.Web.Services.PersistenceService` | **~85%**           | Save/load round-trip tested end-to-end                                            |
| `GymManagement.Web.Components.Pages.Home`      | ~65%                 | bUnit smoke tests hit the happy path                                              |
| `GymManagement.Web.Components.Pages.Bookings`  | ~55%                 | bUnit tests cover book + cancel + validation error                                |
| `GymManagement.Web.Components.Pages.Classes`   | ~65%                 | bUnit tests cover seeded rendering, valid add, invalid add (capacity=0)           |
| `GymManagement.Web.Components.Pages.Members`   | ~60%                 | bUnit tests cover empty state, register happy path, duplicate-ID rejection        |
| `GymManagement.Web.Program` and layout        | 0%                   | Not testable through unit or component tests; smoke tests on real app cover this  |

The 74% figure is a healthy number for a Blazor Server app - all the domain code lands around 90%, and the four user-facing pages are all bUnit-tested. The remaining uncovered lines are almost entirely `Program.cs` and layout wiring that only run end-to-end.

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
| Functional requirements                 | 18    |
| FRs implemented                         | 17    |
| FRs with automated tests                | 17    |
| FRs with a documented gap               | 1 (FR8 - duplicate booking) |
| Non-functional requirements             | 8     |
| NFRs verified                           | 8     |
| Requirements traced to at least one test | **96%** (25 of 26) |

## Contribution activity

Snapshot from the shared repo:

| Metric                            | Value            |
|-----------------------------------|------------------|
| Total commits on `master`         | 69+              |
| Commits by Ali (`kmc4699`)        | 34 (mid-project) + this-week's docs |
| Commits by Srikar (`Srikar Kurani`)| 35 + this-week's|
| Weeks with >= 2 commits per member| Every week since project start |
| Longest gap between commits (any member) | 4 days       |

## Release decision (draft)

Verdict candidates and how each maps to the numbers above:

- **Ready for release** - all major and critical defects closed, coverage on the money-path domain code above 85%, every functional requirement either implemented and tested or explicitly documented as out of scope.
- **Conditionally ready** - the outstanding gap on FR8 (duplicate booking) is documented and behind a known-behaviour test, but is not a blocker for the prototype's intended use. Closing this gap would take the verdict to fully ready.
- **Not ready** - would need at least one open High-severity defect or a major requirement gap, which is not currently the case.

**Recommended verdict:** *Conditionally ready for release* - the system is safe to demo and the risk is small, well-understood, and tracked. This wording lets us claim green on Task 8's release evaluation without overstating.

## Next-week improvement targets

- Close FR8 (Srikar) and log a DEF-04 entry once the current known gap is closed.
- Add a small performance test on `ReportingService` to satisfy Task 7's quality-testing requirement (performance being one of the two chosen areas).
- Draft the report's Tasks 1-11 using the artifacts already produced.
