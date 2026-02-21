using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System;
using System.IO;
using System.Threading.Tasks;

namespace WebAPIBackend.Controllers
{
    [Authorize(Roles = "ADMIN")]
    [Route("api/[controller]")]
    [ApiController]
    public class MigrationController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public MigrationController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("run-witness-history")]
        public async Task<IActionResult> RunWitnessHistoryMigration()
        {
            try
            {
                var connectionString = _configuration.GetSection("connection:connectionString").Value;
                var sqlFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Scripts", "add_witness_history_fields.sql");

                if (!System.IO.File.Exists(sqlFilePath))
                {
                    return NotFound(new { message = "Migration file not found", path = sqlFilePath });
                }

                var sql = await System.IO.File.ReadAllTextAsync(sqlFilePath);

                using (var connection = new NpgsqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        await command.ExecuteNonQueryAsync();
                    }
                }

                return Ok(new { 
                    message = "Witness history migration completed successfully",
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    message = "Migration failed", 
                    error = ex.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }

        [HttpGet("check-witness-history-columns")]
        public async Task<IActionResult> CheckWitnessHistoryColumns()
        {
            try
            {
                var connectionString = _configuration.GetSection("connection:connectionString").Value;
                
                var sql = @"
                    SELECT column_name, data_type 
                    FROM information_schema.columns 
                    WHERE table_schema = 'org' 
                      AND table_name = 'Guarantors' 
                      AND column_name IN ('IsActive', 'ExpiredAt', 'ExpiredBy', 'ReplacedByGuarantorId')
                    ORDER BY column_name;
                ";

                var columns = new System.Collections.Generic.List<object>();

                using (var connection = new NpgsqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new NpgsqlCommand(sql, connection))
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            columns.Add(new
                            {
                                columnName = reader.GetString(0),
                                dataType = reader.GetString(1)
                            });
                        }
                    }
                }

                var allColumnsExist = columns.Count == 4;

                return Ok(new { 
                    migrationApplied = allColumnsExist,
                    columnsFound = columns.Count,
                    expectedColumns = 4,
                    columns = columns
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    message = "Check failed", 
                    error = ex.Message
                });
            }
        }
    }
}
