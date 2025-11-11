using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hyip_Payments.Models
{
    [Table("UserTenant")]
    public class UserTenantModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(128)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(256)]
        public string? Description { get; set; }

        // Optional: Track when the tenant was created
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Optional: Track if the tenant is active
        public bool IsActive { get; set; } = true;

        // Navigation property: Users in this tenant
        public virtual List<UserModel> Users { get; set; } = new();
    }
}

