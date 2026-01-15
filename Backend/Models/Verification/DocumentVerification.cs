using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPIBackend.Models.Verification
{
    /// <summary>
    /// Document Verification Record - Stores verification codes and digital signatures for printed documents
    /// سند تایید - ذخیره کدهای تایید و امضای دیجیتال برای اسناد چاپ شده
    /// </summary>
    [Table("DocumentVerifications", Schema = "org")]
    public class DocumentVerification
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Unique verification code in format: {PREFIX}-{YEAR}-{RANDOM}
        /// Example: LIC-2026-A7X9K2
        /// </summary>
        [Required]
        [MaxLength(20)]
        public string VerificationCode { get; set; } = string.Empty;

        /// <summary>
        /// ID of the document being verified (e.g., LicenseDetail.Id, CompanyDetail.Id)
        /// </summary>
        [Required]
        public int DocumentId { get; set; }

        /// <summary>
        /// Type of document: RealEstateLicense, PetitionWriterLicense, Securities, etc.
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string DocumentType { get; set; } = string.Empty;

        /// <summary>
        /// HMAC-SHA256 digital signature of key document fields
        /// </summary>
        [Required]
        [MaxLength(128)]
        public string DigitalSignature { get; set; } = string.Empty;

        /// <summary>
        /// JSON snapshot of document data at time of verification code generation
        /// </summary>
        [Column(TypeName = "jsonb")]
        public string? DocumentSnapshot { get; set; }

        /// <summary>
        /// Timestamp when verification code was generated
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// User who generated the verification code
        /// </summary>
        [MaxLength(50)]
        public string? CreatedBy { get; set; }

        /// <summary>
        /// Whether this verification has been revoked
        /// </summary>
        public bool IsRevoked { get; set; } = false;

        /// <summary>
        /// Timestamp when verification was revoked
        /// </summary>
        public DateTime? RevokedAt { get; set; }

        /// <summary>
        /// User who revoked the verification
        /// </summary>
        [MaxLength(50)]
        public string? RevokedBy { get; set; }

        /// <summary>
        /// Reason for revocation
        /// </summary>
        [MaxLength(500)]
        public string? RevokedReason { get; set; }
    }
}
