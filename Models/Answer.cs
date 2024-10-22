using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PBL3_Course
{
    [Table("Answer")]
    public class Answer
    {
        [Key]
        public int Id{set;get;}
        [Required(ErrorMessage ="You have not entered content for the answer")]
        public string AnswerText{set;get;}
        public int IsCorrect{set;get;}
        [ForeignKey("QuestionId")]
        public Question? Question{set;get;}
        public int QuestionId{set;get;}
    }
}