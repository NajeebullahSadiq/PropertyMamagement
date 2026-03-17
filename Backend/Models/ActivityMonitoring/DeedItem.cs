using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPIBackend.Models.ActivityMonitoring
{
    /// <summary>
    /// سته‌های اسناد - Deed items with serial numbers for annual report
    /// </summary>
    [Table("ActivityMonitoringDeedItems", Schema = "org")]
    public class DeedItem
    {
        [Key]
        public int Id { get; set; }

        public int ActivityMonitoringRecordId { get; set; }

        /// <summary>
        /// نوعیت سته - Deed type: 1=Vehicle, 2=Rental, 3=Sale, 4=Bai Ul Wafa
        /// </summary>
        public int DeedType { get; set; }

        /// <summary>
        /// آغاز سریال نمبر - Starting serial number
        /// </summary>
        [MaxLength(100)]
        public string? SerialStart { get; set; }

        /// <summary>
        /// ختم سریال نمبر - Ending serial number
        /// </summary>
        [MaxLength(100)]
        public string? SerialEnd { get; set; }

        /// <summary>
        /// تعداد - Quantity of deeds
        /// </summary>
        public int Count { get; set; }

        public DateTime? CreatedAt { get; set; }

        // Foreign key relationship
        [ForeignKey("ActivityMonitoringRecordId")]
        public ActivityMonitoringRecord? ActivityMonitoringRecord { get; set; }
    }
}
