using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PBL3_Course
{
    [Table("Chapter")]
    public class Chapter
    {
        [Key]
        public int Id{set;get;}
        [Required]
        [Display(Name ="Chapter name")]
        public string ChapterName{set;get;}
        public ICollection<Lesson>? lessons{set;get;}
        public Course? Course{set;get;}
        [ForeignKey("CourseId")]
        public int CourseId{set;get;}

    }
}