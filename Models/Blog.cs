using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PBL3_Course
{
    [Table("Blog")]
    public class Blog
    {
        [Key]
        public int Id{set;get;}
        [Required(ErrorMessage = "Bạn chưa nhập tên cho blog cần tạo")]
        public string BlogName{set;get;} ="";

        public ICollection<Post>? posts{set;get;}
    }
}