using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PBL3_Course
{
    [Table("Post")]
    public class Post
    {
        [Key]
        public int Id{set;get;}
        [Required(ErrorMessage ="You have not named the post yet")]
        public string PostName{set;get;}="";
        [Required(ErrorMessage = "You have not entered the content of the post")]
        public string PostContent{set;get;}="";
        public DateTime DateCreatedOrEdited{set;get;}
        [NotMapped]
        [Required(ErrorMessage ="Haven't uploaded a photo for this post yet")]
        public IFormFile FormFile{set;get;}
        public string? ImageLink{set;get;}
        [ForeignKey("BlogId")]
        [Required(ErrorMessage ="You have not chosen a topic for your article")]
        public int BlogId{set;get;}
        public Blog? Blog{set;get;}
    }
}