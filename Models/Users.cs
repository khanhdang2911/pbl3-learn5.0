using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PBL3_Course
{
    [Table("Users")]
    public class Users
    {
        [Key]
        public int Id{set;get;}
        [EmailAddress(ErrorMessage ="Bạn nhập email sai định dạng")]
        [Required(ErrorMessage ="Bạn chưa nhập email")]
        public string Email{set;get;}
        [Required(ErrorMessage ="Bạn chưa nhập tên")]
        public string Name{set;get;}
        [Required(ErrorMessage ="Bạn chưa nhập số điện thoại")]
        [Phone(ErrorMessage ="Nhập số điện thoại sai định dạng")]
        public string Phone{set;get;}
        public string Password{set;get;}
        public ICollection<UsersRole>? usersRoles{set;get;}
        public ICollection<UsersCourse>? usersCourses{set;get;}
        public ICollection<Course>? courses{set;get;}
        public ICollection<Comment>? comments{set;get;}


    }
}