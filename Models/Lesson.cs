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
        
        [Display(Name ="Nội dung bài học")]
        // [Required(ErrorMessage ="Bạn chưa nhập nội dung bài học")]
        public string? FileLinkContent{set;get;}
        [ForeignKey("ChapterId")]
        public int ChapterId{set;get;}
        public Chapter? Chapter{set;get;}

        [Display(Name ="Upload video bài giảng")]
        [NotMapped]
        public IFormFile? FormFile{set;get;}

        [Display(Name ="Upload file tài liệu")]
        [NotMapped]
        public IFormFile? DocumentFile{set;get;}
        public string? DocumentLink{set;get;} 
        public int? IsFree{set;get;}
        
    }
}