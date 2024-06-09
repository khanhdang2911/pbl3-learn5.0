using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PBL3_Course
{
    [Table("Lesson")]
    public class Lesson
    {
        [Key]
        public int Id{set;get;}
        [Required(ErrorMessage ="You have not entered a lesson name")]
        [Display(Name ="Lesson name")]
        public string LessonName{set;get;}

        [DataType(DataType.Text)]
        [Display(Name ="Lesson description")]

        public string? Description{set;get;}
        
        [Display(Name ="Lesson content")]
        // [Required(ErrorMessage ="Bạn chưa nhập nội dung bài học")]
        public string? FileLinkContent{set;get;}
        [ForeignKey("ChapterId")]
        public int ChapterId{set;get;}
        public Chapter? Chapter{set;get;}

        [Display(Name ="Upload lecture videos")]
        [NotMapped]
        public IFormFile? FormFile{set;get;}

        [Display(Name ="Upload document files")]
        [NotMapped]
        public IFormFile? DocumentFile{set;get;}
        public string? DocumentLink{set;get;} 
        public int? IsFree{set;get;}
        public int View{set;get;}=0;
        
    }
}