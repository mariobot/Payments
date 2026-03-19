using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hyip_Payments.Models
{
    /// <summary>
    /// Comprehensive audit log for tracking all system activities
    /// </summary>
    [Table("AuditLogs")]
    public class AuditLogModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// When the action occurred
        /// </summary>
        [Required]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// User ID who performed the action
        /// </summary>
        [MaxLength(450)]
        public string? UserId { get; set; }

        /// <summary>
        /// Username who performed the action
        /// </summary>
        [MaxLength(256)]
        public string? UserName { get; set; }

        /// <summary>
        /// User email
        /// </summary>
        [MaxLength(256)]
        public string? UserEmail { get; set; }

        /// <summary>
        /// Type of action performed (Create, Update, Delete, View, Login, Security, etc.)
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string ActionType { get; set; } = string.Empty;

        /// <summary>
        /// Entity type being audited (Invoice, Payment, Customer, etc.)
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string EntityType { get; set; } = string.Empty;

        /// <summary>
        /// ID of the entity being audited
        /// </summary>
        [MaxLength(100)]
        public string? EntityId { get; set; }

        /// <summary>
        /// IP address of the user
        /// </summary>
        [MaxLength(45)] // IPv6 max length
        public string? IpAddress { get; set; }

        /// <summary>
        /// User agent (browser/device info)
        /// </summary>
        [MaxLength(500)]
        public string? UserAgent { get; set; }

        /// <summary>
        /// Value before the change (JSON format for complex objects)
        /// </summary>
        [Column(TypeName = "nvarchar(max)")]
        public string? BeforeValue { get; set; }

        /// <summary>
        /// Value after the change (JSON format for complex objects)
        /// </summary>
        [Column(TypeName = "nvarchar(max)")]
        public string? AfterValue { get; set; }

        /// <summary>
        /// Severity level (Info, Warning, Critical)
        /// </summary>
        [Required]
        [MaxLength(20)]
        public string Severity { get; set; } = "Info";

        /// <summary>
        /// Whether the action was successful
        /// </summary>
        public bool IsSuccessful { get; set; } = true;

        /// <summary>
        /// Detailed description of the action
        /// </summary>
        [MaxLength(1000)]
        public string? Description { get; set; }

        /// <summary>
        /// Additional details or error messages (JSON format)
        /// </summary>
        [Column(TypeName = "nvarchar(max)")]
        public string? AdditionalData { get; set; }

        /// <summary>
        /// HTTP method (GET, POST, PUT, DELETE)
        /// </summary>
        [MaxLength(10)]
        public string? HttpMethod { get; set; }

        /// <summary>
        /// Request path/URL
        /// </summary>
        [MaxLength(500)]
        public string? RequestPath { get; set; }

        /// <summary>
        /// User role at the time of action
        /// </summary>
        [MaxLength(100)]
        public string? UserRole { get; set; }

        /// <summary>
        /// Duration of the operation in milliseconds
        /// </summary>
        public long? DurationMs { get; set; }

        /// <summary>
        /// Application/Tenant ID for multi-tenancy
        /// </summary>
        public int? ApplicationId { get; set; }

        /// <summary>
        /// Session ID for tracking user sessions
        /// </summary>
        [MaxLength(100)]
        public string? SessionId { get; set; }

        /// <summary>
        /// Correlation ID for distributed tracing
        /// </summary>
        [MaxLength(100)]
        public string? CorrelationId { get; set; }
    }
}
