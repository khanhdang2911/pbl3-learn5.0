using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PBL3_Course
{
    [Table("Category")]
    public class Category
    {
        [Key]
        public int Id{set;get;}
        [Required(ErrorMessage ="Bạn chưa nhập tên lĩnh vực")]
        public string CategoryName{set;get;}
        public ICollection<Course>? courses{set;get;}
    }
}