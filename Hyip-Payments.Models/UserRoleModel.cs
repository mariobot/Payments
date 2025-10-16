using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hyip_Payments.Models
{
    [Table("UserRole")]
    public class UserRoleModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public UserModel User { get; set; } = null!;

        [Required]
        public int RoleId { get; set; }

        [ForeignKey(nameof(RoleId))]
        public RoleModel Role { get; set; } = null!;
    }
}
