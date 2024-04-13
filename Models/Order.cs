using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PBL3_Course
{
    [Table("Order")]
    public class Order
    {
        [Key]
        public int Id{set;get;}
        public int UserId{set;get;}
        public int courseId{set;get;}
        public double TotalMoney{set;get;}

        public DateTime DateCreated{set;get;}
    }
}