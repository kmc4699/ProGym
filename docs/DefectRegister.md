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

Status: Closed - the fix was verified by an automated test and merged into master.