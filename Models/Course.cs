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
        // public int TeacherId{set;get;}
        public ICollection<Lesson>? lessons{set;get;}
        public DateTime DateCreated{set;get;}
        public DateTime? DateEdited{set;get;}

    }
}