# Requirements Document

## Introduction

This feature enables users to enter Dari/Pashto numerals (۰۱۲۳۴۵۶۷۸۹) in number input fields throughout the application. Currently, the application only accepts Western Arabic numerals (0-9), which creates usability issues for users who prefer typing in their native script. The system will accept both numeral systems and normalize them for storage and processing.

## Glossary

- **Western_Arabic_Numerals**: The digits 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 commonly used in Western countries
- **Eastern_Arabic_Numerals**: The digits ۰, ۱, ۲, ۳, ۴, ۵, ۶, ۷, ۸, ۹ used in Dari, Pashto, Persian, and Arabic scripts (also known as Persian/Dari numerals)
- **Number_Input_Field**: Any form input field intended to accept numeric values
- **Numeral_Normalization**: The process of converting Eastern Arabic numerals to Western Arabic numerals for storage and processing
- **Input_Directive**: An Angular directive that intercepts and processes user input on form fields

## Requirements

### Requirement 1: Accept Eastern Arabic Numerals in Input Fields

**User Story:** As a Dari/Pashto speaking user, I want to enter numbers using my native numeral system, so that I can use the application naturally without switching keyboard layouts.

#### Acceptance Criteria

1. WHEN a user types an Eastern Arabic numeral (۰-۹) in a number input field, THE Input_Directive SHALL accept the character and display it in the field
2. WHEN a user types a Western Arabic numeral (0-9) in a number input field, THE Input_Directive SHALL accept the character and display it in the field
3. WHEN a user types a non-numeric character in a number input field, THE Input_Directive SHALL prevent the character from being entered
4. THE Input_Directive SHALL allow decimal points (. or ٫) in fields that support decimal values

### Requirement 2: Normalize Numerals for Data Processing

**User Story:** As a system administrator, I want all numeric data stored in a consistent format, so that calculations and database operations work correctly.

#### Acceptance Criteria

1. WHEN a form containing Eastern Arabic numerals is submitted, THE System SHALL convert all Eastern Arabic numerals to Western Arabic numerals before sending to the backend
2. WHEN numeric data is retrieved from the backend, THE System SHALL display it using the user's preferred numeral system
3. THE Numeral_Normalization SHALL preserve the numeric value exactly (۱۲۳ becomes 123)

### Requirement 3: Replace type="number" with type="text"

**User Story:** As a developer, I want number fields to use text input type with proper validation, so that Eastern Arabic numerals can be entered.

#### Acceptance Criteria

1. THE System SHALL use type="text" instead of type="number" for all numeric input fields
2. WHEN type="text" is used for numeric fields, THE Input_Directive SHALL enforce numeric-only input validation
3. THE Input_Directive SHALL be applied consistently across all numeric input fields in the application

### Requirement 4: Update Existing Number Validation Functions

**User Story:** As a developer, I want a centralized number validation approach, so that the codebase is maintainable and consistent.

#### Acceptance Criteria

1. THE System SHALL provide a shared directive for numeric input validation
2. WHEN the shared directive is applied, THE System SHALL replace all existing onlyNumberKey functions
3. THE Shared_Directive SHALL be importable from the shared module for use across all feature modules

### Requirement 5: Support Mixed Numeral Input

**User Story:** As a user, I want to be able to mix Eastern and Western Arabic numerals in the same field, so that I can type naturally without worrying about consistency.

#### Acceptance Criteria

1. WHEN a user enters a mix of Eastern and Western Arabic numerals (e.g., ۱23۴), THE Input_Directive SHALL accept all characters
2. WHEN mixed numerals are submitted, THE Numeral_Normalization SHALL convert all to Western Arabic numerals (۱23۴ becomes 1234)
