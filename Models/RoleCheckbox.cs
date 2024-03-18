using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PBL3_Course
{
    public class RoleCheckbox
    {
        public int Id{set;get;}
        public string RoleName{set;get;}
        public bool IsChecked{set;get;}
    }
}