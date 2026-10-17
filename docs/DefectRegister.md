# Defect Register - ProGym

## Defect Categories

Severity: High (breaks core functionality or causes data loss), Medium (causes incorrect behaviour in some cases), Low (minor or cosmetic issue)
Priority: P1 (fix immediately), P2 (fix this week), P3 (fix when convenient)
Status: Open, In Progress, Fixed, Verified, Closed

## Defect Lifecycle

1. Open - defect has been identified and logged but has not been worked on
2. In Progress - work on the fix is currently underway
3. Fixed - fix has been implemented and the build succeeds
4. Verified - testing confirms that the fix works and the defect no longer occurs
5. Closed - the fix has been verified and merged into master


## DEF-01: CheckIn.ClassId lost after persistence reload

ID: DEF-01

Found In: Manual code review while integrating the persistence feature with the CheckIn.ClassId field that had been added earlier

Description: When check-in data was saved and then loaded again, the ClassId was reset to null. This affected check-ins that were linked to a class. The issue occurred because CheckInDto did not contain a ClassId field, so the value was not saved to or loaded from the JSON file.

Severity: Medium - the application did not crash, but information was lost after reloading the data. This could affect future features such as attendance reports by class.

Priority: P1 - the issue was fixed on the same day because it affected data integrity (NFR5).

Root Cause: Two related features, CheckIn.ClassId and PersistenceService, were developed separately. The persistence DTO was created before ClassId was added to CheckIn, so the new field was not added to the DTO or persistence logic.

Corrective Action: Added ClassId to CheckInDto, updated ToDto() to save the value, and updated LoadInto() to restore it using the new CheckIn.Restore() method.

Prevention: Added a unit test (CheckIn_Restore_PreservesClassId) to check that ClassId is preserved when a check-in is restored. This means a future change that breaks this behaviour should be detected by automated testing. The team will also communicate changes to shared data structures, such as DTOs, before merging them.

Lifecycle:
- 2026-09-26 - Open: identified during manual review of the first persistence integration pass
- 2026-09-26 - In Progress: Srikar assigned, root cause confirmed as missing DTO field
- 2026-09-26 - Fixed: ClassId added to CheckInDto, CheckIn.Restore factory added, build green locally
- 2026-09-26 - Verified: CheckIn_Restore_PreservesClassId regression test added and passing in CI
- 2026-09-26 - Closed: merged to master, CI green on both runners

Status: Closed - the fix was verified by an automated test and merged into master.

## DEF-02: Data loss when the app is not shut down gracefully

ID: DEF-02

Found In: Manual review of the persistence feature after it was added. We noticed the initial version only saved on `IHostApplicationLifetime.ApplicationStopping`, which does not fire if the process is killed or crashes.

Description: When the persistence layer was first added, saves only happened when the app shut down through the normal ASP.NET lifecycle. Any bookings, member registrations, or check-ins made between startup and a crash (or a hard stop) would be lost from the JSON file. On the next start, the user would see stale data.

Severity: High - the app is meant to survive a restart, but the original design only did so under happy-path conditions. In a real production scenario this would silently lose customer data.

Priority: P1 - the fix went in the same day it was found because it affects data durability (NFR5) and directly undermines the point of persistence.

Root Cause: The persistence design over-relied on a single lifecycle event. The graceful-shutdown hook is best effort, not a guarantee. Because the mid-project prototype had no persistence at all, we underestimated how often a shutdown is ungraceful during dev (Ctrl+C in the console, VS "Stop Debugging", etc.).

Corrective Action: Added a `PersistenceService.Save(store)` call after every state mutation on the Blazor pages - registering a member, renewing a membership, adding a class, recording a booking, cancelling a booking, and checking in. The shutdown hook is still there as a belt-and-braces backup.

Prevention: Added bUnit tests that render the pages and drive the form submissions, then assert the store has the expected data after each submit. These tests would catch a regression where a page stops calling Save after a mutation. In future, any new mutation on a page must be paired with a persistence save call and a bUnit test that exercises it.

Lifecycle:
- 2026-09-22 - Open: identified during review of the initial PersistenceService design
- 2026-09-22 - In Progress: Ali assigned, agreed to move Save calls to every mutation site
- 2026-09-22 - Fixed: Save() wired into RegisterMember, AddClass, Book, Cancel, CheckIn (commit `ced7f88`)
- 2026-09-22 - Verified: Save_ThenLoad_RestoresBookingsAndCancelledState persistence test passing
- 2026-09-22 - Closed: merged to master, CI green on both runners

Status: Closed - fixed on 2026-09-22 in commit `ced7f88`, verified by bUnit tests.


## DEF-03: Blazor Server pages fail at first request because required services are not registered

ID: DEF-03

Found In: The very first time we tried to run the Blazor Web app after adding the Home, Classes and Bookings pages. The browser showed an unhandled exception ("Cannot provide a value for property 'Store' on type 'GymManagement.Web.Components.Pages.Home' because there is no registered service of type 'GymManagement.Web.Services.GymDataStore'").

Description: The four pages used `@inject GymManagement.Web.Services.GymDataStore Store` (and equivalents for `BookingService` and `ReportingService`) but the DI container in `Program.cs` had not been updated to register those types. Every navigation to a page immediately threw a `InvalidOperationException` from the Blazor renderer before any content was returned.

Severity: High - none of the pages worked at all. The app was effectively unusable until the fix.

Priority: P1 - blocking defect; fixed immediately before any further UI work.

Root Cause: The pages were added incrementally while the DI registrations lived in a different file. Nothing in the compilation pipeline linked the `@inject` directive to a corresponding `AddSingleton` call, so a missing registration only shows up at runtime when the page is first requested. There is no compile-time safety net for Blazor DI.

Corrective Action: Added the three missing registrations in `Program.cs`:
```
builder.Services.AddSingleton<GymDataStore>();
builder.Services.AddSingleton<BookingService>();
builder.Services.AddSingleton<ReportingService>();
```
and confirmed all four pages loaded without errors.

Prevention: bUnit page tests (added in Week 2 for Home and Bookings) render each page through a fresh `TestContext`. If a required service is missing, the test fails with the same `InvalidOperationException`. This means we now catch a missing DI registration in the automated test run, not the first time a user opens the browser.

Lifecycle:
- 2026-08-26 - Open: identified on first browser hit to Home page, Blazor renderer threw InvalidOperationException
- 2026-08-26 - In Progress: Ali assigned, root cause confirmed as missing AddSingleton calls in Program.cs
- 2026-08-26 - Fixed: three AddSingleton registrations added to Program.cs (commit `0822466`), all four pages loaded cleanly
- 2026-09-02 - Verified: bUnit page tests added that would reproduce the failure if DI registration regressed
- 2026-09-02 - Closed: merged to master, CI green on both runners

Status: Closed - fixed on 2026-08-26 in commit `0822466`, verified by bUnit page tests added later.

## DEF-04: Waitlist lost after app restart

ID: DEF-04

Found In: Checking the final report against the code. PersistenceService saved members, classes, bookings and check-ins but not the waitlist.

Description: Members on a class waitlist disappeared after a restart, even though the class still showed as full. The waitlist lived in GymDataStore.Waitlist, but it was never written to or read from the JSON file.

Severity: Medium - no crash, but waitlisted members silently lost their place (NFR5).

Priority: P1 - found one day before submission and fixed the same day because it affects data durability.

Root Cause: The waitlist feature was added after the persistence DTOs were written, and nobody added it to the DTOs. Same cause as DEF-01: two features developed separately and a shared data structure changed without updating the persistence layer.

Corrective Action: Added WaitlistDto, saved the waitlist in ToDto(), restored it in LoadInto() in the saved order, and added a WaitlistEntry constructor that keeps the original join time.

Prevention: Added WaitlistPersistenceTests (order and join time restored, empty waitlist stays empty). Rule going forward: when a new list is added to GymDataStore, add it to PersistenceService in the same commit.

Lifecycle:
- 2026-10-17 - Open: found while checking the final report against the code
- 2026-10-17 - In Progress: Srikar assigned, root cause confirmed as missing DTO
- 2026-10-17 - Fixed: waitlist added to PersistenceService, build green locally
- 2026-10-17 - Verified: WaitlistPersistenceTests passing locally and in CI
- 2026-10-17 - Closed: merged to master, CI green on both runners

Status: Closed - fixed and verified on 2026-10-17 by automated tests.