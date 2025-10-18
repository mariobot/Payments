using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hyip_Payments.Models
{
    [Table("User")]
    public class UserModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [MaxLength(256)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(256)]
        public string PasswordHash { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? FirstName { get; set; }

        [MaxLength(100)]
        public string? LastName { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;

        // Navigation property for user roles
        public ICollection<UserRoleModel> UserRoles { get; set; } = new List<UserRoleModel>();
    }
}
