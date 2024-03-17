using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PBL3_Course
{
    [Table("UsersRole")]
    public class UsersRole
    {
        public int UsersId{set;get;}
        public int RoleId{set;get;}
    
        
    }
}