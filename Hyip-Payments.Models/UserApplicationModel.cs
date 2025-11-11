using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hyip_Payments.Models
{
    [Table("UserApplication")]
    public class UserApplicationModel
    {
        private UserTenantModel? userTenant;

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(128)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [MaxLength(256)]
        public string Hash { get; set; } = string.Empty;

        [Required]
        [MaxLength(128)]
        public string Email { get; set; } = string.Empty;

        public bool EmailConfirmed { get; set; } = false;

        [MaxLength(32)]
        public string? PhoneNumber { get; set; }

        public bool PhoneNumberConfirmed { get; set; } = false;

        // Example: Relate to User (if you have a UserModel)
        public int? UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public virtual UserModel? User { get; set; }

        // Example: Relate to UserTenant (if you have multi-tenancy)
        public int? UserTenantId { get; set; }
        [ForeignKey(nameof(UserTenantId))]
        public virtual UserTenantModel? UserTenant { get => userTenant; set => userTenant = value; }

        // Example: Relate to RoleUser (if you have user roles)
        public virtual List<RoleModel> Roles { get; set; } = new();
    }
}
