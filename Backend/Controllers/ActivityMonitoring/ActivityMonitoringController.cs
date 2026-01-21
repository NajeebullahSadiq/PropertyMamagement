using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPIBackend.Configuration;
using WebAPIBackend.Helpers;
using WebAPIBackend.Models.ActivityMonitoring;
using WebAPIBackend.Models.RequestData.ActivityMonitoring;

namespace WebAPIBackend.Controllers.ActivityMonitoring
{
    /// <summary>
    /// Controller for Activity Monitoring (نظارت بر فعالیت دفاتر رهنمای معاملات و عریضه نویسان)
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ActivityMonitoringController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ActivityMonitoringController(AppDbContext context)
        {
            _context = context;
        }

        #region Main Record CRUD

        /// <summary>
        /// Get all activity monitoring records with pagination and search
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] string? calendarType = null)
        {
            try
            {
                var query = _context.ActivityMonitoringRecords
                    .Where(x => x.Status == true)
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(search))
                {
                    query = query.Where(x =>
                        x.LicenseHolderName.Contains(search) ||
                        (x.TaxClearanceLetterNumber != null && x.TaxClearanceLetterNumber.Contains(search)));
                }

                var totalCount = await query.CountAsync();
                var calendar = DateConversionHelper.ParseCalendarType(calendarType);

                var items = await query
                    .OrderByDescending(x => x.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(x => new
                    {
                        x.Id,
                        x.LicenseHolderName,
                        x.TaxClearanceDate,
                        TaxClearanceDateFormatted = x.TaxClearanceDate.HasValue
                            ? DateConversionHelper.FormatDateOnly(x.TaxClearanceDate, calendar)
                            : "",
                        x.PaidTaxAmount,
                        x.ReportRegistrationDate,
                        ReportRegistrationDateFormatted = x.ReportRegistrationDate.HasValue
                            ? DateConversionHelper.FormatDateOnly(x.ReportRegistrationDate, calendar)
                            : "",
                        x.InspectionDate,
                        InspectionDateFormatted = x.InspectionDate.HasValue
                            ? DateConversionHelper.FormatDateOnly(x.InspectionDate, calendar)
                            : "",
                        x.CreatedBy,
                        x.CreatedAt
                    })
                    .ToListAsync();

                return Ok(new
                {
                    items,
                    totalCount,
                    page,
                    pageSize
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Get activity monitoring record by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id, [FromQuery] string? calendarType = null)
        {
            try
            {
                var calendar = DateConversionHelper.ParseCalendarType(calendarType);

                var item = await _context.ActivityMonitoringRecords
                    .Where(x => x.Id == id && x.Status == true)
                    .Select(x => new
                    {
                        x.Id,
                        x.LicenseHolderName,
                        x.TaxClearanceStatus,
                        x.TaxClearanceLetterNumber,
                        x.TaxClearanceDate,
                        TaxClearanceDateFormatted = x.TaxClearanceDate.HasValue
                            ? DateConversionHelper.FormatDateOnly(x.TaxClearanceDate, calendar)
                            : "",
                        x.PaidTaxAmount,
                        x.ReportRegistrationDate,
                        ReportRegistrationDateFormatted = x.ReportRegistrationDate.HasValue
                            ? DateConversionHelper.FormatDateOnly(x.ReportRegistrationDate, calendar)
                            : "",
                        x.SaleDeedsCount,
                        x.RentalDeedsCount,
                        x.BaiUlWafaDeedsCount,
                        x.VehicleTransactionDeedsCount,
                        x.CancelledMixedTransactions,
                        x.LostDeedsCount,
                        x.AnnualReportRemarks,
                        x.InspectionDate,
                        InspectionDateFormatted = x.InspectionDate.HasValue
                            ? DateConversionHelper.FormatDateOnly(x.InspectionDate, calendar)
                            : "",
                        x.InspectedRealEstateOfficesCount,
                        x.SealedOfficesCount,
                        x.InspectedPetitionWritersCount,
                        x.ViolatingPetitionWritersCount,
                        x.Status,
                        x.CreatedAt,
                        x.CreatedBy
                    })
                    .FirstOrDefaultAsync();

                if (item == null)
                {
                    return NotFound("رکورد یافت نشد");
                }

                return Ok(item);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Create new activity monitoring record
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ActivityMonitoringData request)
        {
            try
            {
                var userIdClaim = HttpContext.User.FindFirst("UserID");
                if (userIdClaim == null)
                {
                    return Unauthorized();
                }

                var userId = userIdClaim.Value;

                // Parse dates
                DateConversionHelper.TryParseToDateOnly(request.TaxClearanceDate, request.CalendarType, out var taxClearanceDate);
                DateConversionHelper.TryParseToDateOnly(request.ReportRegistrationDate, request.CalendarType, out var reportRegistrationDate);
                DateConversionHelper.TryParseToDateOnly(request.InspectionDate, request.CalendarType, out var inspectionDate);

                var entity = new ActivityMonitoringRecord
                {
                    LicenseHolderName = request.LicenseHolderName,
                    TaxClearanceStatus = request.TaxClearanceStatus,
                    TaxClearanceLetterNumber = request.TaxClearanceLetterNumber,
                    TaxClearanceDate = taxClearanceDate,
                    PaidTaxAmount = request.PaidTaxAmount,
                    ReportRegistrationDate = reportRegistrationDate,
                    SaleDeedsCount = request.SaleDeedsCount,
                    RentalDeedsCount = request.RentalDeedsCount,
                    BaiUlWafaDeedsCount = request.BaiUlWafaDeedsCount,
                    VehicleTransactionDeedsCount = request.VehicleTransactionDeedsCount,
                    CancelledMixedTransactions = request.CancelledMixedTransactions,
                    LostDeedsCount = request.LostDeedsCount,
                    AnnualReportRemarks = request.AnnualReportRemarks,
                    InspectionDate = inspectionDate,
                    InspectedRealEstateOfficesCount = request.InspectedRealEstateOfficesCount,
                    SealedOfficesCount = request.SealedOfficesCount,
                    InspectedPetitionWritersCount = request.InspectedPetitionWritersCount,
                    ViolatingPetitionWritersCount = request.ViolatingPetitionWritersCount,
                    Status = true,
                    CreatedAt = DateTime.Now,
                    CreatedBy = userId
                };

                _context.ActivityMonitoringRecords.Add(entity);
                await _context.SaveChangesAsync();

                return Ok(new { id = entity.Id, message = "معلومات موفقانه ثبت شد" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Update activity monitoring record
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ActivityMonitoringData request)
        {
            try
            {
                var userIdClaim = HttpContext.User.FindFirst("UserID");
                if (userIdClaim == null)
                {
                    return Unauthorized();
                }

                var userId = userIdClaim.Value;

                var entity = await _context.ActivityMonitoringRecords.FindAsync(id);
                if (entity == null || entity.Status == false)
                {
                    return NotFound("رکورد یافت نشد");
                }

                // Parse dates
                DateConversionHelper.TryParseToDateOnly(request.TaxClearanceDate, request.CalendarType, out var taxClearanceDate);
                DateConversionHelper.TryParseToDateOnly(request.ReportRegistrationDate, request.CalendarType, out var reportRegistrationDate);
                DateConversionHelper.TryParseToDateOnly(request.InspectionDate, request.CalendarType, out var inspectionDate);

                entity.LicenseHolderName = request.LicenseHolderName;
                entity.TaxClearanceStatus = request.TaxClearanceStatus;
                entity.TaxClearanceLetterNumber = request.TaxClearanceLetterNumber;
                entity.TaxClearanceDate = taxClearanceDate;
                entity.PaidTaxAmount = request.PaidTaxAmount;
                entity.ReportRegistrationDate = reportRegistrationDate;
                entity.SaleDeedsCount = request.SaleDeedsCount;
                entity.RentalDeedsCount = request.RentalDeedsCount;
                entity.BaiUlWafaDeedsCount = request.BaiUlWafaDeedsCount;
                entity.VehicleTransactionDeedsCount = request.VehicleTransactionDeedsCount;
                entity.CancelledMixedTransactions = request.CancelledMixedTransactions;
                entity.LostDeedsCount = request.LostDeedsCount;
                entity.AnnualReportRemarks = request.AnnualReportRemarks;
                entity.InspectionDate = inspectionDate;
                entity.InspectedRealEstateOfficesCount = request.InspectedRealEstateOfficesCount;
                entity.SealedOfficesCount = request.SealedOfficesCount;
                entity.InspectedPetitionWritersCount = request.InspectedPetitionWritersCount;
                entity.ViolatingPetitionWritersCount = request.ViolatingPetitionWritersCount;
                entity.UpdatedAt = DateTime.Now;
                entity.UpdatedBy = userId;

                await _context.SaveChangesAsync();

                return Ok(new { id = entity.Id, message = "معلومات موفقانه تغییر یافت" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Delete (soft delete) activity monitoring record
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var entity = await _context.ActivityMonitoringRecords.FindAsync(id);
                if (entity == null)
                {
                    return NotFound("رکورد یافت نشد");
                }

                entity.Status = false;
                await _context.SaveChangesAsync();

                return Ok(new { message = "رکورد موفقانه حذف شد" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        #endregion

        #region Complaints CRUD

        /// <summary>
        /// Get complaints for an activity monitoring record
        /// </summary>
        [HttpGet("{recordId}/complaints")]
        public async Task<IActionResult> GetComplaints(int recordId, [FromQuery] string? calendarType = null)
        {
            try
            {
                var calendar = DateConversionHelper.ParseCalendarType(calendarType);

                var complaints = await _context.ActivityMonitoringComplaints
                    .Where(x => x.ActivityMonitoringRecordId == recordId)
                    .Select(x => new
                    {
                        x.Id,
                        x.ActivityMonitoringRecordId,
                        x.ComplaintSerialNumber,
                        x.ComplainantName,
                        x.ComplaintSubject,
                        x.ComplaintRegistrationDate,
                        ComplaintRegistrationDateFormatted = x.ComplaintRegistrationDate.HasValue
                            ? DateConversionHelper.FormatDateOnly(x.ComplaintRegistrationDate, calendar)
                            : "",
                        x.AccusedPartyName,
                        x.ActionsTaken,
                        x.Remarks,
                        x.CreatedAt
                    })
                    .ToListAsync();

                return Ok(complaints);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Add complaint to activity monitoring record
        /// </summary>
        [HttpPost("{recordId}/complaints")]
        public async Task<IActionResult> AddComplaint(int recordId, [FromBody] ComplaintData request)
        {
            try
            {
                var userIdClaim = HttpContext.User.FindFirst("UserID");
                if (userIdClaim == null)
                {
                    return Unauthorized();
                }

                var userId = userIdClaim.Value;

                // Check if record exists
                var record = await _context.ActivityMonitoringRecords.FindAsync(recordId);
                if (record == null || record.Status == false)
                {
                    return NotFound("رکورد یافت نشد");
                }

                // Parse date
                DateConversionHelper.TryParseToDateOnly(request.ComplaintRegistrationDate, request.CalendarType, out var complaintDate);

                var complaint = new Complaint
                {
                    ActivityMonitoringRecordId = recordId,
                    ComplaintSerialNumber = request.ComplaintSerialNumber,
                    ComplainantName = request.ComplainantName,
                    ComplaintSubject = request.ComplaintSubject,
                    ComplaintRegistrationDate = complaintDate,
                    AccusedPartyName = request.AccusedPartyName,
                    ActionsTaken = request.ActionsTaken,
                    Remarks = request.Remarks,
                    CreatedAt = DateTime.Now,
                    CreatedBy = userId
                };

                _context.ActivityMonitoringComplaints.Add(complaint);
                await _context.SaveChangesAsync();

                return Ok(new { id = complaint.Id, message = "شکایت موفقانه ثبت شد" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Update complaint
        /// </summary>
        [HttpPut("{recordId}/complaints/{complaintId}")]
        public async Task<IActionResult> UpdateComplaint(int recordId, int complaintId, [FromBody] ComplaintData request)
        {
            try
            {
                var complaint = await _context.ActivityMonitoringComplaints
                    .FirstOrDefaultAsync(x => x.Id == complaintId && x.ActivityMonitoringRecordId == recordId);

                if (complaint == null)
                {
                    return NotFound("شکایت یافت نشد");
                }

                // Parse date
                DateConversionHelper.TryParseToDateOnly(request.ComplaintRegistrationDate, request.CalendarType, out var complaintDate);

                complaint.ComplaintSerialNumber = request.ComplaintSerialNumber;
                complaint.ComplainantName = request.ComplainantName;
                complaint.ComplaintSubject = request.ComplaintSubject;
                complaint.ComplaintRegistrationDate = complaintDate;
                complaint.AccusedPartyName = request.AccusedPartyName;
                complaint.ActionsTaken = request.ActionsTaken;
                complaint.Remarks = request.Remarks;

                await _context.SaveChangesAsync();

                return Ok(new { id = complaint.Id, message = "شکایت موفقانه تغییر یافت" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Delete complaint
        /// </summary>
        [HttpDelete("{recordId}/complaints/{complaintId}")]
        public async Task<IActionResult> DeleteComplaint(int recordId, int complaintId)
        {
            try
            {
                var complaint = await _context.ActivityMonitoringComplaints
                    .FirstOrDefaultAsync(x => x.Id == complaintId && x.ActivityMonitoringRecordId == recordId);

                if (complaint == null)
                {
                    return NotFound("شکایت یافت نشد");
                }

                _context.ActivityMonitoringComplaints.Remove(complaint);
                await _context.SaveChangesAsync();

                return Ok(new { message = "شکایت موفقانه حذف شد" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        #endregion

        #region Real Estate Violations CRUD

        /// <summary>
        /// Get real estate violations for an activity monitoring record
        /// </summary>
        [HttpGet("{recordId}/realestate-violations")]
        public async Task<IActionResult> GetRealEstateViolations(int recordId, [FromQuery] string? calendarType = null)
        {
            try
            {
                var calendar = DateConversionHelper.ParseCalendarType(calendarType);

                var violations = await _context.ActivityMonitoringRealEstateViolations
                    .Where(x => x.ActivityMonitoringRecordId == recordId)
                    .Select(x => new
                    {
                        x.Id,
                        x.ActivityMonitoringRecordId,
                        x.ViolationSerialNumber,
                        x.LicenseHolderName,
                        x.ViolationType,
                        x.ViolationDate,
                        ViolationDateFormatted = x.ViolationDate.HasValue
                            ? DateConversionHelper.FormatDateOnly(x.ViolationDate, calendar)
                            : "",
                        x.ActionsTaken,
                        x.Remarks,
                        x.CreatedAt
                    })
                    .ToListAsync();

                return Ok(violations);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Add real estate violation
        /// </summary>
        [HttpPost("{recordId}/realestate-violations")]
        public async Task<IActionResult> AddRealEstateViolation(int recordId, [FromBody] RealEstateViolationData request)
        {
            try
            {
                var userIdClaim = HttpContext.User.FindFirst("UserID");
                if (userIdClaim == null)
                {
                    return Unauthorized();
                }

                var userId = userIdClaim.Value;

                // Check if record exists
                var record = await _context.ActivityMonitoringRecords.FindAsync(recordId);
                if (record == null || record.Status == false)
                {
                    return NotFound("رکورد یافت نشد");
                }

                // Parse date
                DateConversionHelper.TryParseToDateOnly(request.ViolationDate, request.CalendarType, out var violationDate);

                var violation = new RealEstateViolation
                {
                    ActivityMonitoringRecordId = recordId,
                    ViolationSerialNumber = request.ViolationSerialNumber,
                    LicenseHolderName = request.LicenseHolderName,
                    ViolationType = request.ViolationType,
                    ViolationDate = violationDate,
                    ActionsTaken = request.ActionsTaken,
                    Remarks = request.Remarks,
                    CreatedAt = DateTime.Now,
                    CreatedBy = userId
                };

                _context.ActivityMonitoringRealEstateViolations.Add(violation);
                await _context.SaveChangesAsync();

                return Ok(new { id = violation.Id, message = "تخلف موفقانه ثبت شد" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Update real estate violation
        /// </summary>
        [HttpPut("{recordId}/realestate-violations/{violationId}")]
        public async Task<IActionResult> UpdateRealEstateViolation(int recordId, int violationId, [FromBody] RealEstateViolationData request)
        {
            try
            {
                var violation = await _context.ActivityMonitoringRealEstateViolations
                    .FirstOrDefaultAsync(x => x.Id == violationId && x.ActivityMonitoringRecordId == recordId);

                if (violation == null)
                {
                    return NotFound("تخلف یافت نشد");
                }

                // Parse date
                DateConversionHelper.TryParseToDateOnly(request.ViolationDate, request.CalendarType, out var violationDate);

                violation.ViolationSerialNumber = request.ViolationSerialNumber;
                violation.LicenseHolderName = request.LicenseHolderName;
                violation.ViolationType = request.ViolationType;
                violation.ViolationDate = violationDate;
                violation.ActionsTaken = request.ActionsTaken;
                violation.Remarks = request.Remarks;

                await _context.SaveChangesAsync();

                return Ok(new { id = violation.Id, message = "تخلف موفقانه تغییر یافت" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Delete real estate violation
        /// </summary>
        [HttpDelete("{recordId}/realestate-violations/{violationId}")]
        public async Task<IActionResult> DeleteRealEstateViolation(int recordId, int violationId)
        {
            try
            {
                var violation = await _context.ActivityMonitoringRealEstateViolations
                    .FirstOrDefaultAsync(x => x.Id == violationId && x.ActivityMonitoringRecordId == recordId);

                if (violation == null)
                {
                    return NotFound("تخلف یافت نشد");
                }

                _context.ActivityMonitoringRealEstateViolations.Remove(violation);
                await _context.SaveChangesAsync();

                return Ok(new { message = "تخلف موفقانه حذف شد" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        #endregion

        #region Petition Writer Violations CRUD

        /// <summary>
        /// Get petition writer violations for an activity monitoring record
        /// </summary>
        [HttpGet("{recordId}/petitionwriter-violations")]
        public async Task<IActionResult> GetPetitionWriterViolations(int recordId, [FromQuery] string? calendarType = null)
        {
            try
            {
                var calendar = DateConversionHelper.ParseCalendarType(calendarType);

                var violations = await _context.ActivityMonitoringPetitionWriterViolations
                    .Where(x => x.ActivityMonitoringRecordId == recordId)
                    .Select(x => new
                    {
                        x.Id,
                        x.ActivityMonitoringRecordId,
                        x.ViolationSerialNumber,
                        x.PetitionWriterName,
                        x.ViolationType,
                        x.ViolationDate,
                        ViolationDateFormatted = x.ViolationDate.HasValue
                            ? DateConversionHelper.FormatDateOnly(x.ViolationDate, calendar)
                            : "",
                        x.ActionsTaken,
                        x.Remarks,
                        x.CreatedAt
                    })
                    .ToListAsync();

                return Ok(violations);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Add petition writer violation
        /// </summary>
        [HttpPost("{recordId}/petitionwriter-violations")]
        public async Task<IActionResult> AddPetitionWriterViolation(int recordId, [FromBody] PetitionWriterViolationData request)
        {
            try
            {
                var userIdClaim = HttpContext.User.FindFirst("UserID");
                if (userIdClaim == null)
                {
                    return Unauthorized();
                }

                var userId = userIdClaim.Value;

                // Check if record exists
                var record = await _context.ActivityMonitoringRecords.FindAsync(recordId);
                if (record == null || record.Status == false)
                {
                    return NotFound("رکورد یافت نشد");
                }

                // Parse date
                DateConversionHelper.TryParseToDateOnly(request.ViolationDate, request.CalendarType, out var violationDate);

                var violation = new PetitionWriterViolation
                {
                    ActivityMonitoringRecordId = recordId,
                    ViolationSerialNumber = request.ViolationSerialNumber,
                    PetitionWriterName = request.PetitionWriterName,
                    ViolationType = request.ViolationType,
                    ViolationDate = violationDate,
                    ActionsTaken = request.ActionsTaken,
                    Remarks = request.Remarks,
                    CreatedAt = DateTime.Now,
                    CreatedBy = userId
                };

                _context.ActivityMonitoringPetitionWriterViolations.Add(violation);
                await _context.SaveChangesAsync();

                return Ok(new { id = violation.Id, message = "تخلف موفقانه ثبت شد" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Update petition writer violation
        /// </summary>
        [HttpPut("{recordId}/petitionwriter-violations/{violationId}")]
        public async Task<IActionResult> UpdatePetitionWriterViolation(int recordId, int violationId, [FromBody] PetitionWriterViolationData request)
        {
            try
            {
                var violation = await _context.ActivityMonitoringPetitionWriterViolations
                    .FirstOrDefaultAsync(x => x.Id == violationId && x.ActivityMonitoringRecordId == recordId);

                if (violation == null)
                {
                    return NotFound("تخلف یافت نشد");
                }

                // Parse date
                DateConversionHelper.TryParseToDateOnly(request.ViolationDate, request.CalendarType, out var violationDate);

                violation.ViolationSerialNumber = request.ViolationSerialNumber;
                violation.PetitionWriterName = request.PetitionWriterName;
                violation.ViolationType = request.ViolationType;
                violation.ViolationDate = violationDate;
                violation.ActionsTaken = request.ActionsTaken;
                violation.Remarks = request.Remarks;

                await _context.SaveChangesAsync();

                return Ok(new { id = violation.Id, message = "تخلف موفقانه تغییر یافت" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Delete petition writer violation
        /// </summary>
        [HttpDelete("{recordId}/petitionwriter-violations/{violationId}")]
        public async Task<IActionResult> DeletePetitionWriterViolation(int recordId, int violationId)
        {
            try
            {
                var violation = await _context.ActivityMonitoringPetitionWriterViolations
                    .FirstOrDefaultAsync(x => x.Id == violationId && x.ActivityMonitoringRecordId == recordId);

                if (violation == null)
                {
                    return NotFound("تخلف یافت نشد");
                }

                _context.ActivityMonitoringPetitionWriterViolations.Remove(violation);
                await _context.SaveChangesAsync();

                return Ok(new { message = "تخلف موفقانه حذف شد" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        #endregion
    }
}
