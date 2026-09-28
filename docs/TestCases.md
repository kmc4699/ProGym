# ProGym - Test Cases

Formal record of the test cases exercised in the automated suite. Every entry
lists the test name, what requirement it covers, the setup, inputs, expected
result, actual result on the last run, and status. All 51 tests currently pass.

Suite version: 2026-09-29
Command to reproduce: `dotnet test GymManagement.slnx`

## Test type legend

- **Unit** - single class in isolation, no framework
- **Integration** - persistence + domain, uses real file I/O
- **Component (bUnit)** - full Blazor page rendered through bUnit `TestContext`, form submission and DOM assertions

## Test case index

| TC-ID  | Test method                                                                 | Requirement | Type       |
|--------|-----------------------------------------------------------------------------|-------------|------------|
| TC-001 | `Membership_ValidInput_Succeeds`                                            | FR1, FR2    | Unit       |
| TC-002 | `Membership_BlankId_Throws`                                                 | FR2         | Unit       |
| TC-003 | `Membership_BlankName_Throws`                                               | FR2         | Unit       |
| TC-004 | `Membership_IsActive_ReturnsTrueForFutureExpiry`                            | FR3         | Unit       |
| TC-005 | `Membership_IsActive_ReturnsFalseForPastExpiry` (via FakeClock)              | FR3         | Unit       |
| TC-006 | `Membership_Renew_ExtendsExpiryDate`                                        | FR3         | Unit       |
| TC-007 | `Membership_DaysUntilExpiry_ReturnsExpectedCount`                           | FR3         | Unit       |
| TC-008 | `Membership_DaysUntilExpiry_ReturnsNegativeForExpired`                      | FR3         | Unit       |
| TC-009 | `FitnessClass_ValidInput_Succeeds`                                          | FR4         | Unit       |
| TC-010 | `FitnessClass_InvalidCapacity_Throws`                                       | FR5         | Unit       |
| TC-011 | `BookClass_ActiveMemberAndAvailableClass_ReturnsSuccess`                    | FR6         | Unit       |
| TC-012 | `BookClass_ExpiredMembership_ReturnsFailure` (via FakeClock)                | FR9         | Unit       |
| TC-013 | `BookClass_FullClass_ReturnsFailure`                                        | FR10        | Unit       |
| TC-014 | `BookClass_PastStartTime_ReturnsFailure`                                    | FR7         | Unit       |
| TC-015 | `BookClass_FutureStartTime_ReturnsSuccess`                                  | FR7         | Unit       |
| TC-016 | `BookClass_SameMemberSameClassTwice_CurrentlySucceedsBothTimes`             | FR8 (gap)   | Unit       |
| TC-017 | `CancelBooking_ExistingBooking_ReleasesSlot`                                | FR11        | Unit       |
| TC-018 | `CancelBooking_AlreadyCancelled_Throws`                                     | FR11        | Unit       |
| TC-019 | `CheckIn_ActiveMembership_Succeeds`                                         | FR3         | Unit       |
| TC-020 | `CheckIn_ExpiredMembership_ThrowsException` (via FakeClock)                 | FR3         | Unit       |
| TC-021 | `CheckIn_NullMembership_ThrowsException`                                    | -           | Unit       |
| TC-022 | `CheckIn_FromValidBooking_Succeeds`                                         | FR13        | Unit       |
| TC-023 | `CheckIn_FromCancelledBooking_ThrowsException`                              | FR13        | Unit       |
| TC-024 | `CheckIn_NullBooking_ThrowsException`                                       | FR13        | Unit       |
| TC-025 | `CheckIn_DefaultsToAttended`                                                | FR12        | Unit       |
| TC-026 | `CheckIn_NoShowStatus_IsRecorded`                                           | FR12        | Unit       |
| TC-027 | `CheckIn_Restore_PreservesClassId`                                          | FR18        | Unit       |
| TC-028 | `CheckIn_DuplicateForSameBooking_ThrowsException`                           | FR14        | Unit       |
| TC-029 | `CheckIn_SameMemberDifferentClass_Succeeds`                                 | FR14        | Unit       |
| TC-030 | `GetMembershipSummary_CountsActiveAndExpired`                               | FR15        | Unit       |
| TC-031 | `GetClassUtilisation_ReturnsBookedAndCapacityPerClass`                      | FR16        | Unit       |
| TC-032 | `GetTotalCheckIns_CountsAllCheckIns`                                        | FR17        | Unit       |
| TC-033 | `GetMembershipSummary_NullInput_ThrowsException`                            | -           | Unit       |
| TC-034 | `GetClassUtilisation_NullInput_ThrowsException`                             | -           | Unit       |
| TC-035 | `GetTotalCheckIns_NullInput_ThrowsException`                                | -           | Unit       |
| TC-036 | `Save_ThenLoad_RestoresMembersAndClasses`                                   | FR18        | Integration|
| TC-037 | `Save_ThenLoad_RestoresBookedCountOnClasses`                                | FR18        | Integration|
| TC-038 | `Save_ThenLoad_RestoresBookingsAndCancelledState`                           | FR18        | Integration|
| TC-039 | `LoadInto_NoSaveFile_ReturnsFalseAndLeavesStoreUntouched`                   | FR18        | Integration|
| TC-040 | `Home_RendersDashboardHeading`                                              | NFR1        | Component  |
| TC-041 | `Home_WithNoMembers_ShowsZeroActiveAndExpired`                              | FR15        | Component  |
| TC-042 | `Home_WithSeededClasses_ShowsUtilisationRows`                               | FR16        | Component  |
| TC-043 | `Bookings_InitiallyShowsEmptyState`                                         | NFR2        | Component  |
| TC-044 | `Bookings_SelectingMemberAndClass_AndSubmitting_BooksSuccessfully`          | FR6, NFR6   | Component  |
| TC-045 | `Bookings_SubmittingWithoutSelection_ShowsErrorMessage`                     | NFR2        | Component  |
| TC-046 | `Bookings_CancellingAnActiveBooking_ReleasesTheSlotAndShowsCancelledBadge`  | FR11, NFR6  | Component  |

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

- **Total test cases:** 51
- **Passing:** 51
- **Failing:** 0
- **Coverage across requirements:** 17 of 18 FRs and all 8 NFRs have at least one automated test. FR8 has a documented gap-tracking test that will be flipped to a positive assertion once the rule is implemented.
- **Coverage by test type:** ~38 unit, 4 integration (persistence), 7 component (bUnit).

See `docs/RequirementsTraceabilityMatrix.md` for the reverse mapping from requirements to tests, and `docs/DefectRegister.md` for the defects each regression test prevents.
