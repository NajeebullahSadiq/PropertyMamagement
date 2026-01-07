# Requirements Document

## Introduction

This document specifies the requirements for enhancing the Guarantors (تضمین‌کنندگان) section in the Company Module. The enhancement standardizes the Guarantee Type (نوعیت تضمین) dropdown to three specific options and introduces dynamic conditional fields based on the selected guarantee type. This change simplifies the UI, aligns with legal practices in Afghanistan, and ensures data accuracy across frontend, backend, and database layers.

## Glossary

- **Guarantor_System**: The system component responsible for managing guarantor records and their associated guarantee information within the Company Module
- **Guarantee_Type**: A classification of the type of guarantee provided by a guarantor (Cash, Sharia Deed, or Customary Deed)
- **Conditional_Fields**: Form fields that are dynamically shown or hidden based on the selected guarantee type
- **Sharia_Deed**: A legally recognized deed issued by a religious court (قباله شرعی)
- **Customary_Deed**: A deed based on customary/traditional practices (قباله عرفی)
- **Cash_Guarantee**: A monetary guarantee deposited in a bank (پول نقد)
- **Multi_Calendar_System**: The application-wide date system supporting Hijri Qamari, Hijri Shamsi, and Gregorian calendars

## Requirements

### Requirement 1: Guarantee Type Dropdown Standardization

**User Story:** As a system administrator, I want the guarantee type dropdown to contain only three legally recognized options, so that users cannot select invalid or outdated guarantee types.

#### Acceptance Criteria

1. THE Guarantor_System SHALL display exactly three guarantee type options: پول نقد (Cash), قباله شرعی (Sharia Deed), and قباله عرفی (Customary Deed)
2. WHEN the guarantee type dropdown is rendered, THE Guarantor_System SHALL display options with Dari labels only
3. THE Guarantor_System SHALL remove all existing guarantee types from the database lookup table except the three specified options
4. WHEN a user attempts to save a guarantor record, THE Guarantor_System SHALL validate that the selected guarantee type is one of the three allowed options

### Requirement 2: Sharia Deed Conditional Fields

**User Story:** As a data entry operator, I want to see court-related fields only when Sharia Deed is selected, so that I can enter the required legal documentation details.

#### Acceptance Criteria

1. WHEN "قباله شرعی" (Sharia Deed) is selected as the guarantee type, THE Guarantor_System SHALL display the Court Name (محکمه نوم) field
2. WHEN "قباله شرعی" (Sharia Deed) is selected as the guarantee type, THE Guarantor_System SHALL display the Collateral Number (نمبر وثیقه) field
3. WHILE "قباله شرعی" (Sharia Deed) is selected, THE Guarantor_System SHALL require both Court Name and Collateral Number fields to be filled before saving
4. WHEN a different guarantee type is selected after "قباله شرعی", THE Guarantor_System SHALL hide the Court Name and Collateral Number fields
5. WHEN a different guarantee type is selected after "قباله شرعی", THE Guarantor_System SHALL clear any previously entered values in Court Name and Collateral Number fields

### Requirement 3: Customary Deed Conditional Fields

**User Story:** As a data entry operator, I want to see registration-related fields only when Customary Deed is selected, so that I can enter the required traditional documentation details.

#### Acceptance Criteria

1. WHEN "قباله عرفی" (Customary Deed) is selected as the guarantee type, THE Guarantor_System SHALL display the Set Serial Number (نمبر سریال سټه) field
2. WHEN "قباله عرفی" (Customary Deed) is selected as the guarantee type, THE Guarantor_System SHALL display the District/Zone (ناحیه) field
3. WHILE "قباله عرفی" (Customary Deed) is selected, THE Guarantor_System SHALL require both Set Serial Number and District/Zone fields to be filled before saving
4. WHEN a different guarantee type is selected after "قباله عرفی", THE Guarantor_System SHALL hide the Set Serial Number and District/Zone fields
5. WHEN a different guarantee type is selected after "قباله عرفی", THE Guarantor_System SHALL clear any previously entered values in Set Serial Number and District/Zone fields

### Requirement 4: Cash Guarantee Conditional Fields

**User Story:** As a data entry operator, I want to see bank deposit fields only when Cash is selected, so that I can enter the required financial documentation details.

#### Acceptance Criteria

1. WHEN "پول نقد" (Cash) is selected as the guarantee type, THE Guarantor_System SHALL display the Bank Name (بانک) field
2. WHEN "پول نقد" (Cash) is selected as the guarantee type, THE Guarantor_System SHALL display the Deposit/Receipt Number (نمبر اویز) field
3. WHEN "پول نقد" (Cash) is selected as the guarantee type, THE Guarantor_System SHALL display the Deposit Date (تاریخ اویز) field with multi-calendar support
4. WHILE "پول نقد" (Cash) is selected, THE Guarantor_System SHALL require Bank Name, Deposit Number, and Deposit Date fields to be filled before saving
5. WHEN a different guarantee type is selected after "پول نقد", THE Guarantor_System SHALL hide the Bank Name, Deposit Number, and Deposit Date fields
6. WHEN a different guarantee type is selected after "پول نقد", THE Guarantor_System SHALL clear any previously entered values in Bank Name, Deposit Number, and Deposit Date fields

### Requirement 5: Backend Validation

**User Story:** As a system administrator, I want the backend to validate guarantee data based on the selected type, so that inconsistent data combinations cannot be saved.

#### Acceptance Criteria

1. WHEN a guarantor record is submitted with guarantee type "قباله شرعی", THE Guarantor_System SHALL reject the submission if Court Name or Collateral Number is missing
2. WHEN a guarantor record is submitted with guarantee type "قباله عرفی", THE Guarantor_System SHALL reject the submission if Set Serial Number or District/Zone is missing
3. WHEN a guarantor record is submitted with guarantee type "پول نقد", THE Guarantor_System SHALL reject the submission if Bank Name, Deposit Number, or Deposit Date is missing
4. WHEN a guarantor record is submitted, THE Guarantor_System SHALL store only the fields relevant to the selected guarantee type
5. IF a guarantor record is submitted with fields that do not match the selected guarantee type, THEN THE Guarantor_System SHALL ignore the mismatched fields and not store them

### Requirement 6: Database Schema Extension

**User Story:** As a database administrator, I want the guarantors table to support all conditional fields, so that guarantee information can be properly stored.

#### Acceptance Criteria

1. THE Guarantor_System SHALL support storing Court Name as a nullable string field
2. THE Guarantor_System SHALL support storing Collateral Number as a nullable string field
3. THE Guarantor_System SHALL support storing Set Serial Number as a nullable string field
4. THE Guarantor_System SHALL support storing District/Zone as a nullable integer field referencing the Location table
5. THE Guarantor_System SHALL support storing Bank Name as a nullable string field
6. THE Guarantor_System SHALL support storing Deposit Number as a nullable string field
7. THE Guarantor_System SHALL support storing Deposit Date as a nullable date field
8. WHEN database migration is applied, THE Guarantor_System SHALL preserve all existing guarantor records without data loss

### Requirement 7: View and Print Behavior

**User Story:** As a user viewing or printing guarantor records, I want to see only the fields relevant to the selected guarantee type, so that the output is clean and professional.

#### Acceptance Criteria

1. WHEN displaying a guarantor record in view mode, THE Guarantor_System SHALL show only the fields relevant to the stored guarantee type
2. WHEN printing a guarantor record, THE Guarantor_System SHALL clearly display the guarantee type label in Dari
3. WHEN printing a guarantor record, THE Guarantor_System SHALL print only the applicable conditional fields based on the guarantee type
4. WHEN printing a guarantor record, THE Guarantor_System SHALL hide all unused conditional fields completely

### Requirement 8: Data Integrity

**User Story:** As a system administrator, I want the system to prevent saving inconsistent guarantee data, so that all records maintain legal accuracy.

#### Acceptance Criteria

1. THE Guarantor_System SHALL require guarantee type selection before allowing form submission
2. WHEN guarantee type is not selected, THE Guarantor_System SHALL display a validation error message in Dari
3. THE Guarantor_System SHALL validate conditional fields only when their parent guarantee type is selected
4. IF a user attempts to save a record with cash fields while a deed type is selected, THEN THE Guarantor_System SHALL prevent the save operation
5. WHEN editing an existing guarantor record, THE Guarantor_System SHALL load and display the correct conditional fields based on the stored guarantee type
