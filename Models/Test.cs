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
        [Required(ErrorMessage ="Bạn chưa nhập tên cho bài kiểm tra")]
        public string TestName{set;get;}
        [ForeignKey("CourseId")]
        public Course? course{set;get;}
        public int CourseId{set;get;}
        [Range(5,300,ErrorMessage ="Thời gian phải nằm trong khoảng từ 5 phút đến 300 phút")]
        public int Time{set;get;}
        [DisplayName("Number of question")]
        public int NumberOfQuestion{set;get;}

        public List<Question>? Questions{set;get;}
        public List<UsersTest>? usersTests{set;get;}

        
    }
}