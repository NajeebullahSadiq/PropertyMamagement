using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPIBackend.Models.Property
{
    [Table("setadocuments")]
    public class SetaDocument
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        [Column("setanumber")]
        public string SetaNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        [Column("filepath")]
        public string FilePath { get; set; } = string.Empty;

        [StringLength(100)]
        [Column("originalfilename")]
        public string? OriginalFileName { get; set; }

        [StringLength(50)]
        [Column("filetype")]
        public string? FileType { get; set; }

        [Column("filesize")]
        public long FileSize { get; set; }

        [Column("uploadedat")]
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        [Column("uploadedby")]
        public string? UploadedBy { get; set; }
    }
}
