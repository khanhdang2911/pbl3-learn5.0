using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PBL3_Course
{
    [Table("Comment")]
    public class Comment
    {
        [Key]
        public int Id{set;get;}
        [Required(ErrorMessage ="Bạn chưa nhập nội dung phần bình luận")]
        [DataType(DataType.Text)]
        public string CommentText{set;get;}
        public DateTime DateComment{set;get;}
        [ForeignKey("UserId")]
        public int UserId{set;get;}
        [ForeignKey("CourseId")]
        public int CourseId{set;get;}
        public Course? Course{set;get;}
        public Users? User{set;get;}
    }
}