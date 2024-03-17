using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PBL3_Course
{
    [Table("Course")]
    public class Course
    {
        [Key]
        public int Id{set;get;}
        [Required]
        [Display(Name ="Tên khóa học")]
        public string CourseName{set;get;}
        [Required]
        [Display(Name ="Giá")]

        public decimal Price{set;get;}
        [DataType(DataType.Text)]
        [Display(Name ="Mô tả khóa học")]

        public string? Description{set;get;}
        public ICollection<Chapter>? chapters{set;get;}
        public DateTime DateCreated{set;get;}
        public DateTime? DateEdited{set;get;}
        public int status {set;get;}
        [Display(Name ="Ảnh đại diện cho khóa học")]
        [NotMapped]
        public IFormFile? ImageFile{set;get;}
        public string? CourseImageLink{set;get;}
        public int IsActive{set;get;}

        public ICollection<UsersCourse>? usersCourses{set;get;}
    
    }
}