# ProGym - Test Cases

Formal record of the test cases exercised in the automated suite. Every entry
lists the test name, what requirement it covers, the setup, inputs, expected
result, actual result on the last run, and status. All 94 automated tests currently pass. The index below lists 63 of them.

Suite version: 2026-10-17
Command to reproduce: `dotnet test GymManagement.slnx`

## Test type legend

- **Unit** - single class in isolation, no framework
- **Integration** - persistence + domain, uses real file I/O
- **Performance** - repeated operations checked against a one-second time budget
- **Component (bUnit)** - full Blazor page rendered through bUnit `TestContext`, form submission and DOM assertions

## Test case index

- TC-001: `Membership_EmptyMemberId_ThrowsException` (FR2) - Unit
- TC-002: `Membership_PastExpiryDate_ThrowsException` (FR1, FR3) - Unit
- TC-003: `Membership_ExpiryIsToday_IsStillActive` (FR3) - Unit
- TC-004: `Membership_ActiveWhenExpiryInFuture_ReturnsTrue` (FR3) - Unit
- TC-005: `Membership_ExpiredWhenExpiryInPast_ReturnsFalse` (via FakeClock) (FR3) - Unit
- TC-006: `Membership_Renew_ExtendsExpiryDate` (FR3) - Unit
- TC-007: `Membership_DaysUntilExpiry_ReturnsCorrectCount` (FR3) - Unit
- TC-008: `Membership_DaysUntilExpiry_WhenExpired_ReturnsNegativeNumber` (FR3) - Unit
- TC-009: `Constructor_ValidDetails_StartsEmptyWithFullCapacity` (FR4) - Unit
- TC-010: `Constructor_ZeroCapacity_ThrowsException` (FR5) - Unit
- TC-011: `BookClass_ActiveMemberAndAvailableClass_ReturnsSuccess` (FR6) - Unit
- TC-012: `BookClass_ExpiredMembership_ReturnsFailure` (via FakeClock) (FR9) - Unit
- TC-013: `BookClass_FullClass_ReturnsFailure` (FR10) - Unit
- TC-014: `BookClass_PastStartTime_ReturnsFailure` (FR7) - Unit
- TC-015: `BookClass_FutureStartTime_ReturnsSuccess` (FR7) - Unit
- TC-016: `BookClass_DuplicateWithExistingBookingsList_IsRejected` (FR8) - Unit
- TC-017: `CancelBooking_ExistingBooking_ReleasesSlot` (FR11) - Unit
- TC-018: `CancelBooking_AlreadyCancelled_ReturnsFailure` (FR11) - Unit
- TC-019: `CheckIn_ActiveMembership_Succeeds` (FR3) - Unit
- TC-020: `CheckIn_ExpiredMembership_ThrowsException` (via FakeClock) (FR3) - Unit
- TC-021: `CheckIn_NullMembership_ThrowsException` (-) - Unit
- TC-022: `CheckIn_FromValidBooking_Succeeds` (FR13) - Unit
- TC-023: `CheckIn_FromCancelledBooking_ThrowsException` (FR13) - Unit
- TC-024: `CheckIn_NullBooking_ThrowsException` (FR13) - Unit
- TC-025: `CheckIn_DefaultStatus_IsAttended` (FR12) - Unit
- TC-026: `CheckIn_NoShowStatus_IsRecorded` (FR12) - Unit
- TC-027: `CheckIn_Restore_PreservesClassId` (FR18, DEF-01) - Unit
- TC-028: `CheckIn_DuplicateForSameBooking_ThrowsException` (FR14) - Unit
- TC-029: `CheckIn_SameMemberDifferentClass_Succeeds` (FR14) - Unit
- TC-030: `GetMembershipSummary_CountsActiveAndExpired` (FR15) - Unit
- TC-031: `GetClassUtilisation_ReturnsBookedAndCapacityPerClass` (FR16) - Unit
- TC-032: `GetTotalCheckIns_CountsAllCheckIns` (FR17) - Unit
- TC-033: `GetMembershipSummary_NullInput_ThrowsException` (-) - Unit
- TC-034: `GetClassUtilisation_NullInput_ThrowsException` (-) - Unit
- TC-035: `GetTotalCheckIns_NullInput_ThrowsException` (-) - Unit
- TC-036: `Save_ThenLoad_RestoresMembersAndClasses` (FR18) - Integration
- TC-037: `Save_ThenLoad_RestoresBookedCountOnClasses` (FR18) - Integration
- TC-038: `Save_ThenLoad_RestoresBookingsAndCancelledState` (FR18) - Integration
- TC-039: `LoadInto_NoSaveFile_ReturnsFalseAndLeavesStoreUntouched` (FR18) - Integration
- TC-040: `Home_RendersDashboardHeading` (NFR1) - Component
- TC-041: `Home_WithNoMembers_ShowsZeroActiveAndExpired` (FR15) - Component
- TC-042: `Home_WithSeededClasses_ShowsUtilisationRows` (FR16) - Component
- TC-043: `Bookings_InitiallyShowsEmptyState` (NFR2) - Component
- TC-044: `Bookings_SelectingMemberAndClass_AndSubmitting_BooksSuccessfully` (FR6, NFR6) - Component
- TC-045: `Bookings_SubmittingWithoutSelection_ShowsErrorMessage` (NFR2) - Component
- TC-046: `Bookings_CancellingAnActiveBooking_ReleasesTheSlotAndShowsCancelledBadge` (FR11, NFR6) - Component
- TC-047: `CheckIn_Restore_KeepsOriginalCheckInTime` (FR18) - Unit
- TC-048: `CheckIn_Restore_AllowsNullClassId` (FR18) - Unit
- TC-049: `CheckIn_Restore_BlankMemberId_ThrowsException` (FR2, FR18) - Unit
- TC-050: `Membership_RenewAfterExpiry_BecomesActiveAgain` (FR3) - Unit
- TC-051: `Membership_RenewByOneDay_Succeeds` (FR3) - Unit
- TC-052: `Membership_OnExpiryDay_IsActive` (FR3) - Unit
- TC-053: `Membership_DayAfterExpiry_IsNotActive` (FR3) - Unit
- TC-054: `Membership_WhitespaceName_ThrowsException` (FR2) - Unit
- TC-055: `CheckIn_AfterRenewingExpiredMembership_Succeeds` (FR3, FR12) - Integration
- TC-056: `Membership_ManyOperations_CompletesUnderOneSecond` (NFR3) - Performance
- TC-057: `CheckIn_ManyCheckIns_CompletesUnderOneSecond` (NFR3) - Performance
- TC-058: `Save_ThenLoad_RestoresWaitlistOrderAndJoinTime` (FR18, FR22) - Integration
- TC-059: `Save_ThenLoad_EmptyWaitlist_StaysEmpty` (FR18, FR22) - Integration
- TC-060: `BookClass_SameMemberDifferentClass_WithList_Succeeds` (FR8) - Unit
- TC-061: `BookClass_SameClassAfterCancellation_WithList_IsAllowed` (FR8) - Unit
- TC-062: `BookClass_BaseOverload_DoesNotCheckForDuplicates` (FR8) - Unit
- TC-063: `Save_ThenLoad_KeepsCheckInClassId` (FR18, DEF-01) - Integration

## Sample detailed cases

Only the top four are written out in full here as representative examples; the rest live in code and are executed by MSTest under the same discipline.

### TC-011 - `BookClass_ActiveMemberAndAvailableClass_ReturnsSuccess`

- **Requirement:** FR6
- **Precondition:** Fresh `GymDataStore`; one active `Membership` seeded; seeded Yoga class with 10 free slots.
- **Steps:**
  1. Construct a `BookingService`.
  2. Call `BookClass(activeMember, yogaClass)`.
- **Expected result:** `BookingResult.Success == true`; `Booking` object returned with the correct member and class references.
- **Actual result (last run):** As expected.
- **Status:** Pass

### TC-014 - `BookClass_PastStartTime_ReturnsFailure`

- **Requirement:** FR7
- **Precondition:** Fresh `GymDataStore`; one active `Membership`; a `FitnessClass` whose `StartTime` is `DateTime.Now.AddHours(-1)`.
- **Steps:**
  1. Call `BookClass(activeMember, pastClass)`.
- **Expected result:** `Success == false`; message contains "already started"; the class's `BookedCount` stays at 0.
- **Actual result (last run):** As expected.
- **Status:** Pass

### TC-028 - `CheckIn_DuplicateForSameBooking_ThrowsException`

- **Requirement:** FR14
- **Precondition:** Active membership; class with a free slot; one existing successful check-in for this booking in `Store.CheckIns`.
- **Steps:**
  1. Attempt to create another `CheckIn` for the same booking passing the existing check-in list.
- **Expected result:** `InvalidOperationException` with a "already been checked in" message.
- **Actual result (last run):** As expected.
- **Status:** Pass

### TC-044 - `Bookings_SelectingMemberAndClass_AndSubmitting_BooksSuccessfully`

- **Requirement:** FR6, NFR6
- **Precondition:** Bookings page rendered through bUnit `TestContext`; DI provides `GymDataStore` seeded with one member, `BookingService`, `PersistenceService` pointing at a temp file.
- **Steps:**
  1. Change the first `<select>` to the seeded member id.
  2. Change the second `<select>` to the seeded class id.
  3. Submit the form.
- **Expected result:** A `div.alert-success` element appears containing "Booking confirmed"; `Store.Bookings.Count == 1`; the member name appears in the "All Bookings" table markup.
- **Actual result (last run):** As expected.
- **Status:** Pass

## Summary

- **Total automated tests:** 94 (63 listed in the index above)
- **Passing:** 94
- **Failing:** 0
- **Coverage across requirements:** All 23 FRs and all 8 NFRs have at least one automated test.
- **Coverage by test type (by test file):** 55 unit, 22 component (bUnit), 7 integration, 2 performance, 3 boundary, 5 check-in restore.

See `docs/RequirementsTraceabilityMatrix.md` for the reverse mapping from requirements to tests, and `docs/DefectRegister.md` for the defects each regression test prevents.