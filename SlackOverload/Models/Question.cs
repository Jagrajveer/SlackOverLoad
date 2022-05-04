using System.ComponentModel.DataAnnotations.Schema;

namespace SlackOverload.Models {
    public partial class Question {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public DateTime Date { get; set; }

        public virtual ApplicationUser User { get; set; }

        public string UserId { get; set; }
        public virtual ICollection<QuestionTag> Tags { get; set; }

        public virtual ICollection<Answer> Answers { get; set; }
        public virtual ICollection<QuestionComment> QuestionComments { get; set; }

        public Question() {
            Tags = new HashSet<QuestionTag>();
            Answers = new HashSet<Answer>();
            QuestionComments = new HashSet<QuestionComment>();
        }

        public Question(string title, string body, ApplicationUser user) {
            Title = title;
            Body = body;
            User = user;
            UserId = user.Id;
            Date = DateTime.Now;
            Tags = new HashSet<QuestionTag>();
            Answers = new HashSet<Answer>();
            QuestionComments = new HashSet<QuestionComment>();
        }
    }
}
