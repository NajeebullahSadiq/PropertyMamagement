using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPIBackend.Configuration;
using WebAPIBackend.Models.Audit;

namespace WebAPIBackend.Controllers.Audit
{
    /// <summary>
    /// Controller for comprehensive audit log management - Admin only access
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AuditLogController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuditLogController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get paginated audit logs with filtering
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<object>> GetAuditLogs(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? userId = null,
            [FromQuery] string? module = null,
            [FromQuery] string? actionType = null,
            [FromQuery] string? status = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] string? searchTerm = null)
        {
            var query = _context.ComprehensiveAuditLogs.AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(userId))
            {
                query = query.Where(l => l.UserId == userId);
            }

            if (!string.IsNullOrEmpty(module))
            {
                query = query.Where(l => l.Module == module);
            }

            if (!string.IsNullOrEmpty(actionType))
            {
                query = query.Where(l => l.ActionType == actionType);
            }

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(l => l.Status == status);
            }

            if (startDate.HasValue)
            {
                query = query.Where(l => l.Timestamp >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(l => l.Timestamp <= endDate.Value.AddDays(1).AddSeconds(-1));
            }

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(l =>
                    l.Description.Contains(searchTerm) ||
                    l.DescriptionDari != null && l.DescriptionDari.Contains(searchTerm) ||
                    l.UserName != null && l.UserName.Contains(searchTerm) ||
                    l.EntityId != null && l.EntityId.Contains(searchTerm));
            }

            // Get total count
            var totalCount = await query.CountAsync();

            // Get paginated results
            var logs = await query
                .OrderByDescending(l => l.Timestamp)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(l => new
                {
                    l.Id,
                    l.UserId,
                    l.UserName,
                    l.UserRole,
                    l.ActionType,
                    l.Module,
                    l.EntityType,
                    l.EntityId,
                    l.Description,
                    l.DescriptionDari,
                    l.IpAddress,
                    l.RequestUrl,
                    l.HttpMethod,
                    l.Status,
                    l.ErrorMessage,
                    l.UserProvince,
                    l.Timestamp,
                    l.DurationMs
                })
                .ToListAsync();

            return Ok(new
            {
                Data = logs,
                Pagination = new
                {
                    CurrentPage = page,
                    PageSize = pageSize,
                    TotalCount = totalCount,
                    TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
                }
            });
        }

        /// <summary>
        /// Get a specific audit log by ID with full details
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetAuditLogById(long id)
        {
            var log = await _context.ComprehensiveAuditLogs.FindAsync(id);

            if (log == null)
            {
                return NotFound(new { Message = "Audit log not found" });
            }

            return Ok(new
            {
                log.Id,
                log.UserId,
                log.UserName,
                log.UserRole,
                log.ActionType,
                log.Module,
                log.EntityType,
                log.EntityId,
                log.Description,
                log.DescriptionDari,
                log.OldValues,
                log.NewValues,
                log.IpAddress,
                log.UserAgent,
                log.RequestUrl,
                log.HttpMethod,
                log.Status,
                log.ErrorMessage,
                log.Metadata,
                log.UserProvince,
                log.Timestamp,
                log.DurationMs
            });
        }

        /// <summary>
        /// Get audit statistics for dashboard
        /// </summary>
        [HttpGet("statistics")]
        public async Task<ActionResult<object>> GetStatistics([FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null)
        {
            var query = _context.ComprehensiveAuditLogs.AsQueryable();

            // Default to last 30 days if no date range specified
            var effectiveStartDate = startDate ?? DateTime.UtcNow.AddDays(-30);
            var effectiveEndDate = endDate ?? DateTime.UtcNow;

            query = query.Where(l => l.Timestamp >= effectiveStartDate && l.Timestamp <= effectiveEndDate);

            var totalActions = await query.CountAsync();
            var successfulActions = await query.CountAsync(l => l.Status == "Success");
            var failedActions = await query.CountAsync(l => l.Status == "Failed" || l.Status == "Error");

            // Actions by module
            var actionsByModule = await query
                .GroupBy(l => l.Module)
                .Select(g => new { Module = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(10)
                .ToListAsync();

            // Actions by type
            var actionsByType = await query
                .GroupBy(l => l.ActionType)
                .Select(g => new { ActionType = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .ToListAsync();

            // Actions by user (top 10)
            var actionsByUser = await query
                .Where(l => l.UserName != null)
                .GroupBy(l => new { l.UserId, l.UserName })
                .Select(g => new { UserId = g.Key.UserId, UserName = g.Key.UserName, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(10)
                .ToListAsync();

            // Daily activity for chart
            var dailyActivity = await query
                .GroupBy(l => l.Timestamp.Date)
                .Select(g => new { Date = g.Key, Count = g.Count() })
                .OrderBy(x => x.Date)
                .ToListAsync();

            // Recent errors
            var recentErrors = await query
                .Where(l => l.Status == "Error" || l.Status == "Failed")
                .OrderByDescending(l => l.Timestamp)
                .Take(10)
                .Select(l => new
                {
                    l.Id,
                    l.Timestamp,
                    l.UserName,
                    l.Module,
                    l.ActionType,
                    l.Description,
                    l.ErrorMessage
                })
                .ToListAsync();

            return Ok(new
            {
                DateRange = new
                {
                    StartDate = effectiveStartDate,
                    EndDate = effectiveEndDate
                },
                Summary = new
                {
                    TotalActions = totalActions,
                    SuccessfulActions = successfulActions,
                    FailedActions = failedActions,
                    SuccessRate = totalActions > 0 ? Math.Round((double)successfulActions / totalActions * 100, 2) : 0
                },
                ActionsByModule = actionsByModule,
                ActionsByType = actionsByType,
                TopUsers = actionsByUser,
                DailyActivity = dailyActivity,
                RecentErrors = recentErrors
            });
        }

        /// <summary>
        /// Get distinct modules for filtering
        /// </summary>
        [HttpGet("modules")]
        public async Task<ActionResult<IEnumerable<string>>> GetModules()
        {
            var modules = await _context.ComprehensiveAuditLogs
                .Select(l => l.Module)
                .Distinct()
                .OrderBy(m => m)
                .ToListAsync();

            return Ok(modules);
        }

        /// <summary>
        /// Get distinct action types for filtering
        /// </summary>
        [HttpGet("action-types")]
        public async Task<ActionResult<IEnumerable<string>>> GetActionTypes()
        {
            var actionTypes = await _context.ComprehensiveAuditLogs
                .Select(l => l.ActionType)
                .Distinct()
                .OrderBy(a => a)
                .ToListAsync();

            return Ok(actionTypes);
        }

        /// <summary>
        /// Get audit logs for a specific entity
        /// </summary>
        [HttpGet("entity/{entityType}/{entityId}")]
        public async Task<ActionResult<object>> GetEntityAuditHistory(string entityType, string entityId)
        {
            var logs = await _context.ComprehensiveAuditLogs
                .Where(l => l.EntityType == entityType && l.EntityId == entityId)
                .OrderByDescending(l => l.Timestamp)
                .Select(l => new
                {
                    l.Id,
                    l.UserId,
                    l.UserName,
                    l.ActionType,
                    l.Description,
                    l.DescriptionDari,
                    l.OldValues,
                    l.NewValues,
                    l.Status,
                    l.Timestamp
                })
                .ToListAsync();

            return Ok(new
            {
                EntityType = entityType,
                EntityId = entityId,
                History = logs
            });
        }

        /// <summary>
        /// Get user activity history
        /// </summary>
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<object>> GetUserActivity(
            string userId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var query = _context.ComprehensiveAuditLogs
                .Where(l => l.UserId == userId);

            var totalCount = await query.CountAsync();

            var logs = await query
                .OrderByDescending(l => l.Timestamp)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(l => new
                {
                    l.Id,
                    l.ActionType,
                    l.Module,
                    l.EntityType,
                    l.EntityId,
                    l.Description,
                    l.DescriptionDari,
                    l.IpAddress,
                    l.Status,
                    l.Timestamp
                })
                .ToListAsync();

            return Ok(new
            {
                UserId = userId,
                Data = logs,
                Pagination = new
                {
                    CurrentPage = page,
                    PageSize = pageSize,
                    TotalCount = totalCount,
                    TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
                }
            });
        }

        /// <summary>
        /// Export audit logs to CSV format
        /// </summary>
        [HttpGet("export")]
        public async Task<IActionResult> ExportAuditLogs(
            [FromQuery] string? module = null,
            [FromQuery] string? actionType = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var query = _context.ComprehensiveAuditLogs.AsQueryable();

            if (!string.IsNullOrEmpty(module))
                query = query.Where(l => l.Module == module);

            if (!string.IsNullOrEmpty(actionType))
                query = query.Where(l => l.ActionType == actionType);

            if (startDate.HasValue)
                query = query.Where(l => l.Timestamp >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(l => l.Timestamp <= endDate.Value.AddDays(1).AddSeconds(-1));

            var logs = await query
                .OrderByDescending(l => l.Timestamp)
                .Take(10000) // Limit export to 10,000 records
                .ToListAsync();

            var csv = "ID,User ID,User Name,User Role,Action Type,Module,Entity Type,Entity ID,Description,Description Dari,IP Address,Status,Timestamp\n";
            
            foreach (var log in logs)
            {
                csv += $"{log.Id},{log.UserId},{log.UserName ?? ""},{log.UserRole ?? ""},{log.ActionType},{log.Module},{log.EntityType ?? ""},{log.EntityId ?? ""},\"{log.Description.Replace("\"", "\"\"")}\",\"{(log.DescriptionDari ?? "").Replace("\"", "\"\"")}\",{log.IpAddress ?? ""},{log.Status},{log.Timestamp:yyyy-MM-dd HH:mm:ss}\n";
            }

            var bytes = System.Text.Encoding.UTF8.GetBytes(csv);
            return File(bytes, "text/csv", $"audit_logs_{DateTime.UtcNow:yyyyMMdd_HHmmss}.csv");
        }
    }
}
