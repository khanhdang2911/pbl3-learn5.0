using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PBL3_Course.Models;

namespace PBL3_Course
{
    [Table("Test")]
    public class Test
    {

        [Key]
        public int Id{set;get;}
        [Required(ErrorMessage ="You have not entered a name for the test")]
        public string TestName{set;get;}
        [ForeignKey("CourseId")]
        public Course? course{set;get;}
        public int CourseId{set;get;}
        [Range(5,300,ErrorMessage ="The time must be between 5 minutes and 300 minutes")]
        public int Time{set;get;}
        [DisplayName("Number of question")]
        public int NumberOfQuestion{set;get;}

        public List<Question>? Questions{set;get;}
        public List<UsersTest>? usersTests{set;get;}

        
    }
}