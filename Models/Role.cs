using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PBL3_Course
{
    [Table("Role")]
    public class Role
    {
        [Key]
        public int Id{set;get;}
        [Required(ErrorMessage ="You have not entered a role name")]
        public string RoleName{set;get;}
        public ICollection<UsersRole>? usersRoles{set;get;}
    }
}