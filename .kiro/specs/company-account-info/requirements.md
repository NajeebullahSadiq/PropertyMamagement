# Requirements Document

## Introduction

This document defines the requirements for adding a new Account / Financial Information section (مالیه) to the Company module. This section will capture key financial and accounting data for each company, including tax settlement information, payment details, transaction counts, and commission data. The feature will be fully integrated into the existing Company module workflow without affecting current functionality.

## Glossary

- **Company_Module**: The existing real estate company management module that handles company details, owners, licenses, and guarantors
- **Account_Info**: A new entity/section that stores financial and accounting information linked to a company
- **Settlement_Info**: Text description of account settlement details (نمرمكتوب تصفيه معلومات)
- **Tax_Payment**: The monetary amount paid for taxes (تحويل ماليات)
- **Settlement_Year**: The fiscal year for which the settlement applies (سال تصفيه مالية)
- **Tax_Payment_Date**: The date when tax payment was made (تاريخ تحويل ماليات)
- **Transaction_Count**: Number of transactions processed (تعدادی معامله)
- **Company_Commission**: Commission amount for the company (كمیشن رهنما)
- **Multi_Calendar_System**: The existing system supporting Georgian, Hijri Qamari, and Hijri Shamsi calendars

## Requirements

### Requirement 1: Account Info Data Model

**User Story:** As a system administrator, I want to store financial information for each company, so that I can track tax settlements and commissions.

#### Acceptance Criteria

1. THE Account_Info entity SHALL have a foreign key relationship to the Company entity
2. THE Account_Info entity SHALL include a settlement_info field of type text for storing settlement description
3. THE Account_Info entity SHALL include a tax_payment_amount field of type decimal for storing currency values
4. THE Account_Info entity SHALL include a settlement_year field of type integer for storing the fiscal year
5. THE Account_Info entity SHALL include a tax_payment_date field of type date for storing payment dates
6. THE Account_Info entity SHALL include a transaction_count field of type integer for storing transaction numbers
7. THE Account_Info entity SHALL include a company_commission field of type decimal for storing commission amounts
8. THE Account_Info entity SHALL include standard audit fields (CreatedAt, CreatedBy, Status)
9. WHEN an Account_Info record is created, THE System SHALL automatically populate CreatedAt with the current timestamp

### Requirement 2: Account Info API Endpoints

**User Story:** As a frontend developer, I want API endpoints to manage account information, so that I can integrate the feature into the UI.

#### Acceptance Criteria

1. THE Backend SHALL expose a GET endpoint to retrieve account info by company ID
2. THE Backend SHALL expose a POST endpoint to create new account info records
3. THE Backend SHALL expose a PUT endpoint to update existing account info records
4. WHEN a POST request is received with invalid numeric values, THE Backend SHALL return a 400 Bad Request with validation errors
5. WHEN a GET request is received for a non-existent company, THE Backend SHALL return a 404 Not Found response
6. THE Backend SHALL validate that tax_payment_amount is a non-negative decimal value
7. THE Backend SHALL validate that transaction_count is a non-negative integer value
8. THE Backend SHALL validate that company_commission is a non-negative decimal value

### Requirement 3: Account Info UI Section

**User Story:** As a company administrator, I want a dedicated Account Info tab in the Company module, so that I can view and manage financial information.

#### Acceptance Criteria

1. THE Frontend SHALL display an Account Info tab (مالیه) in the Company module tab group
2. THE Account_Info tab SHALL be disabled until a company is selected or created
3. THE Account_Info tab SHALL display a form with all six financial fields
4. WHEN the form is in view mode, THE System SHALL display all fields as read-only
5. WHEN the form is in edit mode, THE System SHALL allow modification of all fields
6. THE Form SHALL display field labels in Dari language
7. THE Form SHALL follow the existing Tailwind-based styling patterns used in the Company module

### Requirement 4: Date Field Calendar Support

**User Story:** As a user, I want to enter dates using my preferred calendar system, so that I can work with familiar date formats.

#### Acceptance Criteria

1. THE tax_payment_date field SHALL use the system-wide multi-calendar date picker component
2. THE date picker SHALL support Georgian calendar selection
3. THE date picker SHALL support Hijri Qamari calendar selection
4. THE date picker SHALL support Hijri Shamsi calendar selection
5. WHEN a date is selected in any calendar format, THE System SHALL store it in a standardized format in the database

### Requirement 5: Form Validation

**User Story:** As a user, I want clear validation feedback, so that I can enter correct data.

#### Acceptance Criteria

1. WHEN a user enters non-numeric characters in the tax_payment_amount field, THE System SHALL display a validation error
2. WHEN a user enters non-numeric characters in the transaction_count field, THE System SHALL display a validation error
3. WHEN a user enters non-numeric characters in the company_commission field, THE System SHALL display a validation error
4. WHEN a user enters text exceeding 500 characters in settlement_info, THE System SHALL display a validation error
5. THE tax_payment_amount field SHALL be required
6. THE settlement_info, settlement_year, tax_payment_date, transaction_count, and company_commission fields SHALL be optional

### Requirement 6: Print Integration

**User Story:** As a user, I want to print company information including financial data, so that I can have physical records.

#### Acceptance Criteria

1. WHEN printing company details, THE System SHALL include the Account Info section
2. THE printed Account Info section SHALL display all fields with Dari labels
3. THE printed Account Info section SHALL be formatted as a professional table layout
4. WHEN Account Info data is empty, THE System SHALL display appropriate placeholder text in the print view

### Requirement 7: Database Migration

**User Story:** As a database administrator, I want the new table created safely, so that existing data is not affected.

#### Acceptance Criteria

1. THE migration SHALL create a new CompanyAccountInfo table in the org schema
2. THE migration SHALL NOT modify or delete any existing company records
3. THE migration SHALL create appropriate foreign key constraints to CompanyDetails
4. THE migration SHALL be reversible (support rollback)
5. WHEN the migration runs on an existing database, THE System SHALL preserve all existing data
