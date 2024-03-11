using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PBL3_Course
{
    [Table("Lesson")]
    public class Lesson
    {
        [Key]
        public int Id{set;get;}
        [Required(ErrorMessage ="Bạn chưa nhập tên bài học")]
        [Display(Name ="Tên bài học")]
        public string LessonName{set;get;}

        [DataType(DataType.Text)]
        [Display(Name ="Mô tả bài học")]

        public string? Description{set;get;}
        // public int TeacherId{set;get;}
        [Display(Name ="Nội dung bài học")]
        [Required(ErrorMessage ="Bạn chưa nhập tên bài học")]
        public string FileLinkContent{set;get;}
        [ForeignKey("CourseId")]
        public int CourseId{set;get;}
        public Course? Course{set;get;}
    }
}