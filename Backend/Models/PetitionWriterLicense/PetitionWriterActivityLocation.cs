using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPIBackend.Models.PetitionWriterLicense
{
    /// <summary>
    /// محل فعالیت عریضه‌نویس - Admin configurable dropdown options
    /// </summary>
    [Table("PetitionWriterActivityLocations", Schema = "org")]
    public class PetitionWriterActivityLocationEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(500)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string DariName { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        // Audit Fields
        public DateTime? CreatedAt { get; set; }

        [MaxLength(50)]
        public string? CreatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }

        [MaxLength(50)]
        public string? UpdatedBy { get; set; }
    }
}
