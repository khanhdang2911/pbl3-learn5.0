using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PBL3_Course
{
    [Table("UsersCourse")]
    public class UsersCourse
    {
        public int UsersId{set;get;}
        public int CourseId{set;get;}
    
        
    }
}