using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PBL3_Course
{
    [Table("Post")]
    public class Post
    {
        [Key]
        public int Id{set;get;}
        [Required(ErrorMessage ="Bạn chưa đặt tên cho bài post")]
        public string PostName{set;get;}="";
        [Required(ErrorMessage = "Bạn chưa nhập nội dung của bài post")]
        public string PostContent{set;get;}="";
        public DateTime DateCreatedOrEdited{set;get;}
        public string ImageLink{set;get;}="";
        [ForeignKey("BlogId")]
        public int BlogId{set;get;}
        public Blog? Blog{set;get;}
    }
}