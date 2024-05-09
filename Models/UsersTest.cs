using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PBL3_Course.Models
{
    [Table("UsersTest")]
    public class UsersTest
    {
        public int UsersId{set;get;}
        public int TestId{set;get;}
        public DateTime DateSubmited{set;get;}
        public int correctAnswer{set;get;}
    }
}