using System;
using System.Globalization;
using Npgsql;

namespace DataMigration
{
    /// <summary>
    /// Converts all Hijri Shamsi dates in the database to Gregorian dates
    /// This ensures the database only contains Gregorian dates for proper sorting and filtering
    /// </summary>
    public class ConvertDatesToGregorian
    {
        private readonly string _connectionString;
        private readonly PersianCalendar _persianCalendar = new PersianCalendar();

        public ConvertDatesToGregorian(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Convert a date from Hijri Shamsi to Gregorian
        /// Returns null if the date is already Gregorian or invalid
        /// </summary>
        private DateOnly? ConvertHijriToGregorian(DateOnly date)
        {
            try
            {
                // Check if date is in Hijri Shamsi range (1300-1500)
                if (date.Year < 1300 || date.Year > 1500)
                {
                    // Already Gregorian, return null to indicate no conversion needed
                    return null;
                }

                // Validate the Hijri Shamsi date
                if (date.Month < 1 || date.Month > 12 || date.Day < 1)
                {
                    Console.WriteLine($"Invalid Hijri date: {date}");
                    return null;
                }

                // Get days in month for validation
                int maxDays;
                if (date.Month <= 6)
                {
                    maxDays = 31;
                }
                else if (date.Month <= 11)
                {
                    maxDays = 30;
                }
                else
                {
                    maxDays = _persianCalendar.IsLeapYear(date.Year) ? 30 : 29;
                }

                if (date.Day > maxDays)
                {
                    Console.WriteLine($"Invalid day {date.Day} for Hijri month {date.Month} year {date.Year}");
                    return null;
                }

                // Convert Hijri Shamsi to Gregorian
                var gregorianDate = _persianCalendar.ToDateTime(date.Year, date.Month, date.Day, 0, 0, 0, 0);
                return DateOnly.FromDateTime(gregorianDate);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error converting date {date}: {ex.Message}");
                return null;
            }
        }

        public async Task ConvertAllDates()
        {
            Console.WriteLine("=".PadRight(60, '='));
            Console.WriteLine("Converting All Hijri Shamsi Dates to Gregorian");
            Console.WriteLine("=".PadRight(60, '='));
            Console.WriteLine();

            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            // Convert LicenseDetails dates
            await ConvertLicenseDetailsDates(connection);

            // Convert CompanyOwner dates
            await ConvertCompanyOwnerDates(connection);

            // Convert Guarantors dates
            await ConvertGuarantorsDates(connection);

            // Convert CompanyCancellationInfo dates
            await ConvertCancellationInfoDates(connection);

            // Convert CompanyAccountInfo dates
            await ConvertAccountInfoDates(connection);

            Console.WriteLine();
            Console.WriteLine("=".PadRight(60, '='));
            Console.WriteLine("Conversion Complete!");
            Console.WriteLine("=".PadRight(60, '='));
        }

        private async Task ConvertLicenseDetailsDates(NpgsqlConnection connection)
        {
            Console.WriteLine("Converting LicenseDetails dates...");
            int convertedCount = 0;
            int skippedCount = 0;

            var selectQuery = @"
                SELECT ""Id"", ""IssueDate"", ""ExpireDate"", ""RoyaltyDate"", ""PenaltyDate"", ""HrLetterDate""
                FROM org.""LicenseDetails""
                WHERE ""IssueDate"" IS NOT NULL OR ""ExpireDate"" IS NOT NULL 
                   OR ""RoyaltyDate"" IS NOT NULL OR ""PenaltyDate"" IS NOT NULL 
                   OR ""HrLetterDate"" IS NOT NULL";

            await using var selectCmd = new NpgsqlCommand(selectQuery, connection);
            await using var reader = await selectCmd.ExecuteReaderAsync();

            var updates = new List<(int id, DateOnly? issueDate, DateOnly? expireDate, DateOnly? royaltyDate, DateOnly? penaltyDate, DateOnly? hrLetterDate)>();

            while (await reader.ReadAsync())
            {
                int id = reader.GetInt32(0);
                DateOnly? issueDate = reader.IsDBNull(1) ? null : reader.GetFieldValue<DateOnly>(1);
                DateOnly? expireDate = reader.IsDBNull(2) ? null : reader.GetFieldValue<DateOnly>(2);
                DateOnly? royaltyDate = reader.IsDBNull(3) ? null : reader.GetFieldValue<DateOnly>(3);
                DateOnly? penaltyDate = reader.IsDBNull(4) ? null : reader.GetFieldValue<DateOnly>(4);
                DateOnly? hrLetterDate = reader.IsDBNull(5) ? null : reader.GetFieldValue<DateOnly>(5);

                bool needsUpdate = false;
                DateOnly? newIssueDate = issueDate;
                DateOnly? newExpireDate = expireDate;
                DateOnly? newRoyaltyDate = royaltyDate;
                DateOnly? newPenaltyDate = penaltyDate;
                DateOnly? newHrLetterDate = hrLetterDate;

                if (issueDate.HasValue)
                {
                    var converted = ConvertHijriToGregorian(issueDate.Value);
                    if (converted.HasValue)
                    {
                        newIssueDate = converted.Value;
                        needsUpdate = true;
                        Console.WriteLine($"  License {id}: IssueDate {issueDate.Value} -> {converted.Value}");
                    }
                }

                if (expireDate.HasValue)
                {
                    var converted = ConvertHijriToGregorian(expireDate.Value);
                    if (converted.HasValue)
                    {
                        newExpireDate = converted.Value;
                        needsUpdate = true;
                        Console.WriteLine($"  License {id}: ExpireDate {expireDate.Value} -> {converted.Value}");
                    }
                }

                if (royaltyDate.HasValue)
                {
                    var converted = ConvertHijriToGregorian(royaltyDate.Value);
                    if (converted.HasValue)
                    {
                        newRoyaltyDate = converted.Value;
                        needsUpdate = true;
                    }
                }

                if (penaltyDate.HasValue)
                {
                    var converted = ConvertHijriToGregorian(penaltyDate.Value);
                    if (converted.HasValue)
                    {
                        newPenaltyDate = converted.Value;
                        needsUpdate = true;
                    }
                }

                if (hrLetterDate.HasValue)
                {
                    var converted = ConvertHijriToGregorian(hrLetterDate.Value);
                    if (converted.HasValue)
                    {
                        newHrLetterDate = converted.Value;
                        needsUpdate = true;
                    }
                }

                if (needsUpdate)
                {
                    updates.Add((id, newIssueDate, newExpireDate, newRoyaltyDate, newPenaltyDate, newHrLetterDate));
                    convertedCount++;
                }
                else
                {
                    skippedCount++;
                }
            }

            await reader.CloseAsync();

            // Perform updates
            foreach (var update in updates)
            {
                var updateQuery = @"
                    UPDATE org.""LicenseDetails""
                    SET ""IssueDate"" = @issueDate,
                        ""ExpireDate"" = @expireDate,
                        ""RoyaltyDate"" = @royaltyDate,
                        ""PenaltyDate"" = @penaltyDate,
                        ""HrLetterDate"" = @hrLetterDate
                    WHERE ""Id"" = @id";

                await using var updateCmd = new NpgsqlCommand(updateQuery, connection);
                updateCmd.Parameters.AddWithValue("id", update.id);
                updateCmd.Parameters.AddWithValue("issueDate", update.issueDate.HasValue ? (object)update.issueDate.Value : DBNull.Value);
                updateCmd.Parameters.AddWithValue("expireDate", update.expireDate.HasValue ? (object)update.expireDate.Value : DBNull.Value);
                updateCmd.Parameters.AddWithValue("royaltyDate", update.royaltyDate.HasValue ? (object)update.royaltyDate.Value : DBNull.Value);
                updateCmd.Parameters.AddWithValue("penaltyDate", update.penaltyDate.HasValue ? (object)update.penaltyDate.Value : DBNull.Value);
                updateCmd.Parameters.AddWithValue("hrLetterDate", update.hrLetterDate.HasValue ? (object)update.hrLetterDate.Value : DBNull.Value);

                await updateCmd.ExecuteNonQueryAsync();
            }

            Console.WriteLine($"  ✓ Converted {convertedCount} records, skipped {skippedCount} (already Gregorian)");
            Console.WriteLine();
        }

        private async Task ConvertCompanyOwnerDates(NpgsqlConnection connection)
        {
            Console.WriteLine("Converting CompanyOwner dates...");
            int convertedCount = 0;
            int skippedCount = 0;

            var selectQuery = @"
                SELECT ""Id"", ""DateofBirth""
                FROM org.""CompanyOwner""
                WHERE ""DateofBirth"" IS NOT NULL";

            await using var selectCmd = new NpgsqlCommand(selectQuery, connection);
            await using var reader = await selectCmd.ExecuteReaderAsync();

            var updates = new List<(int id, DateOnly dateOfBirth)>();

            while (await reader.ReadAsync())
            {
                int id = reader.GetInt32(0);
                DateOnly dateOfBirth = reader.GetFieldValue<DateOnly>(1);

                var converted = ConvertHijriToGregorian(dateOfBirth);
                if (converted.HasValue)
                {
                    updates.Add((id, converted.Value));
                    convertedCount++;
                    Console.WriteLine($"  Owner {id}: DateOfBirth {dateOfBirth} -> {converted.Value}");
                }
                else
                {
                    skippedCount++;
                }
            }

            await reader.CloseAsync();

            // Perform updates
            foreach (var update in updates)
            {
                var updateQuery = @"
                    UPDATE org.""CompanyOwner""
                    SET ""DateofBirth"" = @dateOfBirth
                    WHERE ""Id"" = @id";

                await using var updateCmd = new NpgsqlCommand(updateQuery, connection);
                updateCmd.Parameters.AddWithValue("id", update.id);
                updateCmd.Parameters.AddWithValue("dateOfBirth", update.dateOfBirth);

                await updateCmd.ExecuteNonQueryAsync();
            }

            Console.WriteLine($"  ✓ Converted {convertedCount} records, skipped {skippedCount} (already Gregorian)");
            Console.WriteLine();
        }

        private async Task ConvertGuarantorsDates(NpgsqlConnection connection)
        {
            Console.WriteLine("Converting Guarantors dates...");
            int convertedCount = 0;
            int skippedCount = 0;

            var selectQuery = @"
                SELECT ""Id"", ""PropertyDocumentDate"", ""SenderMaktobDate"", ""AnswerdMaktobDate"",
                       ""DateofGuarantee"", ""GuaranteeDate"", ""DepositDate""
                FROM org.""Guarantors""
                WHERE ""PropertyDocumentDate"" IS NOT NULL OR ""SenderMaktobDate"" IS NOT NULL 
                   OR ""AnswerdMaktobDate"" IS NOT NULL OR ""DateofGuarantee"" IS NOT NULL
                   OR ""GuaranteeDate"" IS NOT NULL OR ""DepositDate"" IS NOT NULL";

            await using var selectCmd = new NpgsqlCommand(selectQuery, connection);
            await using var reader = await selectCmd.ExecuteReaderAsync();

            var updates = new List<(int id, DateOnly? propDocDate, DateOnly? senderDate, DateOnly? answeredDate, 
                                     DateOnly? dateOfGuarantee, DateOnly? guaranteeDate, DateOnly? depositDate)>();

            while (await reader.ReadAsync())
            {
                int id = reader.GetInt32(0);
                DateOnly? propDocDate = reader.IsDBNull(1) ? null : reader.GetFieldValue<DateOnly>(1);
                DateOnly? senderDate = reader.IsDBNull(2) ? null : reader.GetFieldValue<DateOnly>(2);
                DateOnly? answeredDate = reader.IsDBNull(3) ? null : reader.GetFieldValue<DateOnly>(3);
                DateOnly? dateOfGuarantee = reader.IsDBNull(4) ? null : reader.GetFieldValue<DateOnly>(4);
                DateOnly? guaranteeDate = reader.IsDBNull(5) ? null : reader.GetFieldValue<DateOnly>(5);
                DateOnly? depositDate = reader.IsDBNull(6) ? null : reader.GetFieldValue<DateOnly>(6);

                bool needsUpdate = false;
                DateOnly? newPropDocDate = propDocDate;
                DateOnly? newSenderDate = senderDate;
                DateOnly? newAnsweredDate = answeredDate;
                DateOnly? newDateOfGuarantee = dateOfGuarantee;
                DateOnly? newGuaranteeDate = guaranteeDate;
                DateOnly? newDepositDate = depositDate;

                if (propDocDate.HasValue)
                {
                    var converted = ConvertHijriToGregorian(propDocDate.Value);
                    if (converted.HasValue) { newPropDocDate = converted.Value; needsUpdate = true; }
                }

                if (senderDate.HasValue)
                {
                    var converted = ConvertHijriToGregorian(senderDate.Value);
                    if (converted.HasValue) { newSenderDate = converted.Value; needsUpdate = true; }
                }

                if (answeredDate.HasValue)
                {
                    var converted = ConvertHijriToGregorian(answeredDate.Value);
                    if (converted.HasValue) { newAnsweredDate = converted.Value; needsUpdate = true; }
                }

                if (dateOfGuarantee.HasValue)
                {
                    var converted = ConvertHijriToGregorian(dateOfGuarantee.Value);
                    if (converted.HasValue) { newDateOfGuarantee = converted.Value; needsUpdate = true; }
                }

                if (guaranteeDate.HasValue)
                {
                    var converted = ConvertHijriToGregorian(guaranteeDate.Value);
                    if (converted.HasValue) { newGuaranteeDate = converted.Value; needsUpdate = true; }
                }

                if (depositDate.HasValue)
                {
                    var converted = ConvertHijriToGregorian(depositDate.Value);
                    if (converted.HasValue) { newDepositDate = converted.Value; needsUpdate = true; }
                }

                if (needsUpdate)
                {
                    updates.Add((id, newPropDocDate, newSenderDate, newAnsweredDate, newDateOfGuarantee, newGuaranteeDate, newDepositDate));
                    convertedCount++;
                }
                else
                {
                    skippedCount++;
                }
            }

            await reader.CloseAsync();

            // Perform updates
            foreach (var update in updates)
            {
                var updateQuery = @"
                    UPDATE org.""Guarantors""
                    SET ""PropertyDocumentDate"" = @propDocDate,
                        ""SenderMaktobDate"" = @senderDate,
                        ""AnswerdMaktobDate"" = @answeredDate,
                        ""DateofGuarantee"" = @dateOfGuarantee,
                        ""GuaranteeDate"" = @guaranteeDate,
                        ""DepositDate"" = @depositDate
                    WHERE ""Id"" = @id";

                await using var updateCmd = new NpgsqlCommand(updateQuery, connection);
                updateCmd.Parameters.AddWithValue("id", update.id);
                updateCmd.Parameters.AddWithValue("propDocDate", update.propDocDate.HasValue ? (object)update.propDocDate.Value : DBNull.Value);
                updateCmd.Parameters.AddWithValue("senderDate", update.senderDate.HasValue ? (object)update.senderDate.Value : DBNull.Value);
                updateCmd.Parameters.AddWithValue("answeredDate", update.answeredDate.HasValue ? (object)update.answeredDate.Value : DBNull.Value);
                updateCmd.Parameters.AddWithValue("dateOfGuarantee", update.dateOfGuarantee.HasValue ? (object)update.dateOfGuarantee.Value : DBNull.Value);
                updateCmd.Parameters.AddWithValue("guaranteeDate", update.guaranteeDate.HasValue ? (object)update.guaranteeDate.Value : DBNull.Value);
                updateCmd.Parameters.AddWithValue("depositDate", update.depositDate.HasValue ? (object)update.depositDate.Value : DBNull.Value);

                await updateCmd.ExecuteNonQueryAsync();
            }

            Console.WriteLine($"  ✓ Converted {convertedCount} records, skipped {skippedCount} (already Gregorian)");
            Console.WriteLine();
        }

        private async Task ConvertCancellationInfoDates(NpgsqlConnection connection)
        {
            Console.WriteLine("Converting CompanyCancellationInfo dates...");
            int convertedCount = 0;
            int skippedCount = 0;

            var selectQuery = @"
                SELECT ""Id"", ""LicenseCancellationLetterDate""
                FROM org.""CompanyCancellationInfo""
                WHERE ""LicenseCancellationLetterDate"" IS NOT NULL";

            await using var selectCmd = new NpgsqlCommand(selectQuery, connection);
            await using var reader = await selectCmd.ExecuteReaderAsync();

            var updates = new List<(int id, DateOnly cancellationDate)>();

            while (await reader.ReadAsync())
            {
                int id = reader.GetInt32(0);
                DateOnly cancellationDate = reader.GetFieldValue<DateOnly>(1);

                var converted = ConvertHijriToGregorian(cancellationDate);
                if (converted.HasValue)
                {
                    updates.Add((id, converted.Value));
                    convertedCount++;
                }
                else
                {
                    skippedCount++;
                }
            }

            await reader.CloseAsync();

            // Perform updates
            foreach (var update in updates)
            {
                var updateQuery = @"
                    UPDATE org.""CompanyCancellationInfo""
                    SET ""LicenseCancellationLetterDate"" = @cancellationDate
                    WHERE ""Id"" = @id";

                await using var updateCmd = new NpgsqlCommand(updateQuery, connection);
                updateCmd.Parameters.AddWithValue("id", update.id);
                updateCmd.Parameters.AddWithValue("cancellationDate", update.cancellationDate);

                await updateCmd.ExecuteNonQueryAsync();
            }

            Console.WriteLine($"  ✓ Converted {convertedCount} records, skipped {skippedCount} (already Gregorian)");
            Console.WriteLine();
        }

        private async Task ConvertAccountInfoDates(NpgsqlConnection connection)
        {
            Console.WriteLine("Converting CompanyAccountInfo dates...");
            int convertedCount = 0;
            int skippedCount = 0;

            var selectQuery = @"
                SELECT ""Id"", ""TaxPaymentDate""
                FROM org.""CompanyAccountInfo""
                WHERE ""TaxPaymentDate"" IS NOT NULL";

            await using var selectCmd = new NpgsqlCommand(selectQuery, connection);
            await using var reader = await selectCmd.ExecuteReaderAsync();

            var updates = new List<(int id, DateOnly taxPaymentDate)>();

            while (await reader.ReadAsync())
            {
                int id = reader.GetInt32(0);
                DateOnly taxPaymentDate = reader.GetFieldValue<DateOnly>(1);

                var converted = ConvertHijriToGregorian(taxPaymentDate);
                if (converted.HasValue)
                {
                    updates.Add((id, converted.Value));
                    convertedCount++;
                }
                else
                {
                    skippedCount++;
                }
            }

            await reader.CloseAsync();

            // Perform updates
            foreach (var update in updates)
            {
                var updateQuery = @"
                    UPDATE org.""CompanyAccountInfo""
                    SET ""TaxPaymentDate"" = @taxPaymentDate
                    WHERE ""Id"" = @id";

                await using var updateCmd = new NpgsqlCommand(updateQuery, connection);
                updateCmd.Parameters.AddWithValue("id", update.id);
                updateCmd.Parameters.AddWithValue("taxPaymentDate", update.taxPaymentDate);

                await updateCmd.ExecuteNonQueryAsync();
            }

            Console.WriteLine($"  ✓ Converted {convertedCount} records, skipped {skippedCount} (already Gregorian)");
            Console.WriteLine();
        }
    }
}
