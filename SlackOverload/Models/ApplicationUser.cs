using Microsoft.AspNetCore.Identity;

namespace SlackOverload.Models {
    public class ApplicationUser : IdentityUser {
        public int Reputation { get; set; } = 0;

        public virtual ICollection<Question> Questions { get; set; }
        public virtual ICollection<Answer> Answers { get; set; }
        public virtual ICollection<QuestionComment> QuestionComments { get; set; }
        public virtual ICollection<AnswerComment> AnswerComments { get; set; }

        public ApplicationUser() { 
            Questions = new HashSet<Question>();
            Answers = new HashSet<Answer>();
            QuestionComments = new HashSet<QuestionComment>();
            AnswerComments = new HashSet<AnswerComment>();
        }
    }
}
