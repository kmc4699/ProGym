# ProGym - Requirements Traceability Matrix

This document maps every requirement from the mid-project report to the code
that implements it, the automated tests that cover it, and any related defect
entries. It is updated on every material change to requirements or code.

Version: Final phase (Assessment 2), updated 2026-10-07
Test suite: 90 automated tests, 0 failing on the last successful CI run

## Legend

- **Status:** Implemented / Partially / Not implemented / Out of scope
- **Test evidence:** the automated tests that cover the requirement (see `docs/TestCases.md` for full details)
- **Defect refs:** related defect IDs in `docs/DefectRegister.md`

## Functional requirements

| Req  | Summary                                                              | Status         | Implemented in                                | Test evidence                                                                                    | Defect refs |
|------|----------------------------------------------------------------------|----------------|-----------------------------------------------|--------------------------------------------------------------------------------------------------|-------------|
| FR1  | Register a new member with a unique ID                                | Implemented    | `Membership` ctor, `Members.razor.RegisterMember` | `Membership_ValidInput_Succeeds`; duplicate-ID guard covered by manual verification              |             |
| FR2  | Reject blank member ID or name                                        | Implemented    | `Membership` ctor validation                   | `Membership_BlankId_Throws`, `Membership_BlankName_Throws`                                        |             |
| FR3  | Track membership expiry (active vs expired)                           | Implemented    | `Membership.IsActive()`                        | `Membership_IsActive_ReturnsTrueForFutureExpiry`, `Membership_IsActive_ReturnsFalseForPastExpiry` |             |
| FR4  | Create a class with capacity and start time                           | Implemented    | `FitnessClass` ctor                            | `FitnessClass_ValidInput_Succeeds`                                                               |             |
| FR5  | Reject a class with zero or negative capacity                         | Implemented    | `FitnessClass` ctor validation                 | `FitnessClass_InvalidCapacity_Throws`                                                            |             |
| FR6  | Book an active member into an available class                         | Implemented    | `BookingService.BookClass`                     | `BookClass_ActiveMemberAndAvailableClass_ReturnsSuccess`, `Bookings_Submitting_BooksSuccessfully` |             |
| FR7  | Reject bookings for classes whose start time has already passed       | Implemented    | `BookingService.BookClass`                     | `BookClass_PastStartTime_ReturnsFailure`, `BookClass_FutureStartTime_ReturnsSuccess`             |             |
| FR8  | Prevent the same member from booking the same class twice             | Implemented    | `BookingService.BookClass(member, class, existing)` overload | `BookClass_DuplicateWithExistingBookingsList_IsRejected`, `BookClass_SameMemberDifferentClass_WithList_Succeeds`, `BookClass_SameClassAfterCancellation_WithList_IsAllowed` |             |
| FR9  | Reject bookings for expired members                                   | Implemented    | `BookingService.BookClass`                     | `BookClass_ExpiredMembership_ReturnsFailure`                                                     |             |
| FR10 | Reject bookings for fully booked classes                              | Implemented    | `BookingService.BookClass` + `FitnessClass.HasAvailableSlot` | `BookClass_FullClass_ReturnsFailure`                                                             |             |
| FR11 | Cancel a booking and free up the slot                                 | Implemented    | `BookingService.CancelBooking`                 | `CancelBooking_ExistingBooking_ReleasesSlot`, `Bookings_Cancelling_ReleasesSlot`                 |             |
| FR12 | Record attendance status (Attended / NoShow) on check-in              | Implemented    | `CheckIn.Status`                               | `CheckIn_DefaultsToAttended`, `CheckIn_NoShowStatus_IsRecorded`                                   |             |
| FR13 | Only allow check-in for a confirmed booking                           | Implemented    | `CheckIn(Booking)` overload                    | `CheckIn_FromValidBooking_Succeeds`, `CheckIn_FromCancelledBooking_ThrowsException`               |             |
| FR14 | Prevent duplicate check-ins for the same member and class             | Implemented    | `CheckIn(Booking, IEnumerable<CheckIn>)` overload | `CheckIn_DuplicateForSameBooking_ThrowsException`, `CheckIn_SameMemberDifferentClass_Succeeds`   |             |
| FR15 | Show active vs expired member counts                                  | Implemented    | `ReportingService.GetMembershipSummary`        | `GetMembershipSummary_CountsActiveAndExpired`                                                    |             |
| FR16 | Show class utilisation (booked out of capacity)                       | Implemented    | `ReportingService.GetClassUtilisation`         | `GetClassUtilisation_ReturnsBookedAndCapacityPerClass`                                            |             |
| FR17 | Show total check-in count                                             | Implemented    | `ReportingService.GetTotalCheckIns`            | `GetTotalCheckIns_CountsAllCheckIns`                                                             |             |
| FR18 | Persist data across app restarts                                      | Implemented    | `PersistenceService.Save`/`LoadInto`, save-on-mutation | `Save_ThenLoad_RestoresMembersAndClasses`, `Save_ThenLoad_RestoresBookedCountOnClasses`, `Save_ThenLoad_RestoresBookingsAndCancelledState` | DEF-01, DEF-02 |
| FR19 | Attendance breakdown per class (attended / no-show)                  | Implemented    | `ReportingService.GetAttendanceByClass`, Dashboard columns | `GetAttendanceByClass_MixedStatuses_CountsCorrectlyPerClass`, `Home_ShowsAttendedAndNoShowColumnsWithBadges` | |
| FR20 | Visual expiry warning for members within 14 days of expiry            | Implemented    | Members page badge logic + `Membership.DaysUntilExpiry` | `Members_ExpiringSoonMember_ShowsYellowExpiringSoonBadge`, `Members_ActiveMemberWithLongExpiry_ShowsGreenActiveBadge` | |
| FR21 | Filter class list by name and optionally hide past classes            | Implemented    | Classes page filter inputs                     | `Classes_FilterByName_ShowsOnlyMatchingRows`, `Classes_FilterWithNoMatches_ShowsNoResultsMessage`, `Classes_HidePastCheckbox_ExcludesClassesInThePast` | |
| FR22 | Waitlist for full classes with auto-promotion on cancellation         | Implemented    | `WaitlistService.Join` / `PromoteNext`, Bookings page integration | `Join_FullClassAndValidMember_ReturnsSuccessAndEntry`, `PromoteNext_OldestWaitlisterGetsBooked`, `Bookings_JoinWaitlist_WhenClassFull_AddsToWaitlist`, `Bookings_CancellingBooking_AutoPromotesOldestWaitlister` | |
| FR23 | CSV export of bookings                                                | Implemented    | `Bookings.BuildBookingsCsv`, download via JS interop | `BuildBookingsCsv_ActiveAndCancelled_ProducesExpectedRows` | |

## Non-functional requirements

| Req   | Summary                                                                     | Status         | How verified                                                                                                     | Defect refs |
|-------|------------------------------------------------------------------------------|----------------|-------------------------------------------------------------------------------------------------------------------|-------------|
| NFR1  | Web UI accessible on any modern desktop browser                              | Implemented    | Manually verified on Chrome and Edge; Blazor Server output is standard HTML                                        |             |
| NFR2  | Every action returns a clear success or error message on the same page       | Implemented    | Covered by every page test (`Bookings_Submitting_...`, `Bookings_SubmittingWithoutSelection_...`, etc.)            |             |
| NFR3  | Business rules live inside domain classes, not the UI                        | Implemented    | Design review: `BookingService`, `Membership`, `FitnessClass`, `CheckIn` own their validation                     |             |
| NFR4  | Automated regression coverage                                                | Implemented    | 90 MSTest + bUnit + integration + performance tests, run on every push via GitHub Actions (`ci.yml`)              |             |
| NFR5  | Data must survive an app restart (no silent loss)                            | Implemented    | Persistence with save-on-mutation and shutdown hook; round-trip tested in `PersistenceServiceTests`               | DEF-01, DEF-02 |
| NFR6  | Every mutation must be observable through the UI                             | Implemented    | bUnit tests drive the pages and assert both the rendered markup and the store state                              |             |
| NFR7  | Development workflow catches regressions before merge                        | Implemented    | GitHub Actions CI blocks PRs with failing tests; code coverage reported per run                                    |             |
| NFR8  | Team must be able to reproduce any bug from a defect entry                   | Implemented    | Defect register (`docs/DefectRegister.md`) links each defect to its commit, root cause, and prevention test       | DEF-01/02/03 |

## Summary

- **23 functional requirements** total; **all 23 implemented** - FR8 was closed on 2026-10-07, and FR19-FR23 were added for the final-phase features (attendance breakdown, expiry warnings, class filter, waitlist, CSV export).
- **8 non-functional requirements** total; all implemented and verified.
- **3 defects logged**, all Closed, each with an automated regression test that prevents recurrence.
- **90 automated tests, 0 failing**; CI green on the last push (2026-10-07).
- **Requirements traced to at least one automated test:** 100% (every FR and NFR has test evidence).

No remaining FR gaps for the Assessment 2 submission. Future improvements are tracked in `docs/QualityMetrics.md` and in the final report (Task 10).
