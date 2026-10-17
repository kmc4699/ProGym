cd C:\Users\srika\ProGym
@'
# ProGym - Requirements Traceability Matrix

This document maps every requirement to the code that implements it, the
automated tests that cover it, and any related defect entries. It is updated
on every material change to requirements or code.

Note: requirement numbers were revised after the mid-project report. The
mid-project report had 16 functional requirements; this final version has 23
(class creation, persistence and five new features were added) and a revised
list of 8 non-functional requirements.

Version: Final phase (Assessment 2), updated 2026-10-17
Test suite: 94 automated tests, 0 failing on the last local run

## How to read this

- Status: Implemented / Partially / Not implemented / Out of scope
- Tests: the automated tests that cover the requirement (see `docs/TestCases.md`)
- Defects: related defect IDs in `docs/DefectRegister.md`

## Functional requirements

- FR1: Register a new member with a unique ID
  - Status: Implemented
  - Code: `Membership` constructor, `Members.razor` RegisterMember
  - Tests: `Membership_ActiveWhenExpiryInFuture_ReturnsTrue`, `Members_RegisterForm_AddsMemberAndShowsInTable`, `Members_RegisterForm_DuplicateId_ShowsErrorAndDoesNotAdd`
  - Defects: none

- FR2: Reject blank member ID or name
  - Status: Implemented
  - Code: `Membership` constructor validation
  - Tests: `Membership_EmptyMemberId_ThrowsException`, `Membership_WhitespaceName_ThrowsException`, `CheckIn_Restore_BlankMemberId_ThrowsException`
  - Defects: none

- FR3: Track membership expiry (active vs expired)
  - Status: Implemented
  - Code: `Membership.IsActive()`, `Membership.Renew()`
  - Tests: `Membership_ActiveWhenExpiryInFuture_ReturnsTrue`, `Membership_ExpiredWhenExpiryInPast_ReturnsFalse`, `Membership_OnExpiryDay_IsActive`, `Membership_DayAfterExpiry_IsNotActive`, `Membership_RenewAfterExpiry_BecomesActiveAgain`
  - Defects: none

- FR4: Create a class with capacity and start time
  - Status: Implemented
  - Code: `FitnessClass` constructor
  - Tests: `Constructor_ValidDetails_StartsEmptyWithFullCapacity`
  - Defects: none

- FR5: Reject a class with zero or negative capacity
  - Status: Implemented
  - Code: `FitnessClass` constructor validation
  - Tests: `Constructor_ZeroCapacity_ThrowsException`
  - Defects: none

- FR6: Book an active member into an available class
  - Status: Implemented
  - Code: `BookingService.BookClass`
  - Tests: `BookClass_ActiveMemberAndAvailableClass_ReturnsSuccess`, `Bookings_SelectingMemberAndClass_AndSubmitting_BooksSuccessfully`
  - Defects: none

- FR7: Reject bookings for classes whose start time has already passed
  - Status: Implemented
  - Code: `BookingService.BookClass`
  - Tests: `BookClass_PastStartTime_ReturnsFailure`, `BookClass_FutureStartTime_ReturnsSuccess`
  - Defects: none

- FR8: Prevent the same member from booking the same class twice
  - Status: Implemented
  - Code: `BookingService.BookClass(member, class, existing)` overload
  - Tests: `BookClass_DuplicateWithExistingBookingsList_IsRejected`, `BookClass_SameMemberDifferentClass_WithList_Succeeds`, `BookClass_SameClassAfterCancellation_WithList_IsAllowed`
  - Defects: none

- FR9: Reject bookings for expired members
  - Status: Implemented
  - Code: `BookingService.BookClass`
  - Tests: `BookClass_ExpiredMembership_ReturnsFailure`
  - Defects: none

- FR10: Reject bookings for fully booked classes
  - Status: Implemented
  - Code: `BookingService.BookClass`, `FitnessClass.HasAvailableSlot`
  - Tests: `BookClass_FullClass_ReturnsFailure`
  - Defects: none

- FR11: Cancel a booking and free up the slot
  - Status: Implemented
  - Code: `BookingService.CancelBooking`
  - Tests: `CancelBooking_ExistingBooking_ReleasesSlot`, `Bookings_CancellingAnActiveBooking_ReleasesTheSlotAndShowsCancelledBadge`
  - Defects: none

- FR12: Record attendance status (Attended / NoShow) on check-in
  - Status: Implemented
  - Code: `CheckIn.Status`
  - Tests: `CheckIn_DefaultStatus_IsAttended`, `CheckIn_NoShowStatus_IsRecorded`
  - Defects: none

- FR13: Only allow check-in for a confirmed booking
  - Status: Implemented
  - Code: `CheckIn(Booking)` constructor
  - Tests: `CheckIn_FromValidBooking_Succeeds`, `CheckIn_FromCancelledBooking_ThrowsException`
  - Defects: none

- FR14: Prevent duplicate check-ins for the same member and class
  - Status: Implemented
  - Code: `CheckIn(Booking, IEnumerable<CheckIn>)` constructor
  - Tests: `CheckIn_DuplicateForSameBooking_ThrowsException`, `CheckIn_SameMemberDifferentClass_Succeeds`
  - Defects: none

- FR15: Show active vs expired member counts
  - Status: Implemented
  - Code: `ReportingService.GetMembershipSummary`
  - Tests: `GetMembershipSummary_CountsActiveAndExpired`
  - Defects: none

- FR16: Show class utilisation (booked out of capacity)
  - Status: Implemented
  - Code: `ReportingService.GetClassUtilisation`
  - Tests: `GetClassUtilisation_ReturnsBookedAndCapacityPerClass`
  - Defects: none

- FR17: Show total check-in count
  - Status: Implemented
  - Code: `ReportingService.GetTotalCheckIns`
  - Tests: `GetTotalCheckIns_CountsAllCheckIns`
  - Defects: none

- FR18: Persist data across app restarts
  - Status: Implemented
  - Code: `PersistenceService.Save` and `LoadInto`, save after every change
  - Tests: `Save_ThenLoad_RestoresMembersAndClasses`, `Save_ThenLoad_RestoresBookedCountOnClasses`, `Save_ThenLoad_RestoresBookingsAndCancelledState`, `CheckIn_Restore_PreservesClassId`, `Save_ThenLoad_KeepsCheckInClassId`, `Save_ThenLoad_RestoresWaitlistOrderAndJoinTime`
  - Defects: DEF-01, DEF-02, DEF-04

- FR19: Attendance breakdown per class (attended / no-show)
  - Status: Implemented
  - Code: `ReportingService.GetAttendanceByClass`, Dashboard columns
  - Tests: `GetAttendanceByClass_MixedStatuses_CountsCorrectlyPerClass`, `Home_ShowsAttendedAndNoShowColumnsWithBadges`
  - Defects: none

- FR20: Visual expiry warning for members within 14 days of expiry
  - Status: Implemented
  - Code: Members page badge logic, `Membership.DaysUntilExpiry`
  - Tests: `Members_ExpiringSoonMember_ShowsYellowExpiringSoonBadge`, `Members_ActiveMemberWithLongExpiry_ShowsGreenActiveBadge`
  - Defects: none

- FR21: Filter class list by name and optionally hide past classes
  - Status: Implemented
  - Code: Classes page filter inputs
  - Tests: `Classes_FilterByName_ShowsOnlyMatchingRows`, `Classes_FilterWithNoMatches_ShowsNoResultsMessage`, `Classes_HidePastCheckbox_ExcludesClassesInThePast`
  - Defects: none

- FR22: Waitlist for full classes with auto-promotion on cancellation
  - Status: Implemented
  - Code: `WaitlistService.Join` and `PromoteNext`, Bookings page
  - Tests: `Join_FullClassAndValidMember_ReturnsSuccessAndEntry`, `PromoteNext_OldestWaitlisterGetsBooked`, `Bookings_JoinWaitlist_WhenClassFull_AddsToWaitlist`, `Bookings_CancellingBooking_AutoPromotesOldestWaitlister`, `Save_ThenLoad_RestoresWaitlistOrderAndJoinTime`
  - Defects: DEF-04

- FR23: CSV export of bookings
  - Status: Implemented
  - Code: `Bookings.BuildBookingsCsv`, download through JS interop
  - Tests: `BuildBookingsCsv_ActiveAndCancelled_ProducesExpectedRows`
  - Defects: none

## Non-functional requirements

- NFR1: Web UI accessible on any modern desktop browser
  - Status: Implemented
  - How verified: manually checked in a desktop browser; Blazor Server output is standard HTML
  - Defects: none

- NFR2: Every action returns a clear success or error message on the same page
  - Status: Implemented
  - How verified: page tests, for example `Bookings_SubmittingWithoutSelection_ShowsErrorMessage`
  - Defects: none

- NFR3: Single-user operations complete in under one second (performance)
  - Status: Implemented
  - How verified: `Membership_ManyOperations_CompletesUnderOneSecond`, `CheckIn_ManyCheckIns_CompletesUnderOneSecond` (domain operations only)
  - Defects: none

- NFR4: Business rules live inside the domain classes, not the UI, and are covered by automated regression tests
  - Status: Implemented
  - How verified: design review (`BookingService`, `Membership`, `FitnessClass` and `CheckIn` own their validation); 94 automated tests run on every push through GitHub Actions (`ci.yml`)
  - Defects: none

- NFR5: Data must survive an app restart (no silent loss)
  - Status: Implemented
  - How verified: save after every change; round trips tested in `PersistenceServiceTests` and `WaitlistPersistenceTests`
  - Defects: DEF-01, DEF-02, DEF-04

- NFR6: Every change must be visible through the UI
  - Status: Implemented
  - How verified: bUnit tests drive the pages and check both the rendered markup and the store state
  - Defects: none

- NFR7: The development workflow catches regressions early
  - Status: Implemented
  - How verified: GitHub Actions runs the full suite on every push to master on Linux and Windows, and reports coverage; failures show on the commit and the README badge
  - Defects: none

- NFR8: The team can reproduce any bug from its defect entry
  - Status: Implemented
  - How verified: the defect register links each defect to its root cause, fix and prevention test
  - Defects: DEF-01, DEF-02, DEF-03, DEF-04

## Summary

- 23 functional requirements, all implemented. FR8 was closed on 2026-10-07, and FR19 to FR23 were added for the final-phase features (attendance breakdown, expiry warnings, class filter, waitlist, CSV export).
- 8 non-functional requirements, all implemented and verified.
- 4 defects logged, all Closed, each with an automated regression test.
- 94 automated tests, 0 failing (2026-10-17).
- Requirements traced to at least one automated test or documented check: 100%.

No remaining requirement gaps for the Assessment 2 submission. Future improvements are tracked in `docs/QualityMetrics.md` and in the final report (Task 10).

'@ | Set-Content -Path docs\RequirementsTraceabilityMatrix.md -Encoding ASCII
git status