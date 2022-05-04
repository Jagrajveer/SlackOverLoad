namespace SlackOverload.Models {
    public partial class Answer {
        public int Id { get; set; }
        public string Body { get; set; }
        public DateTime Date { get; set; }
        public int? QuestionId { get; set; }
        public string UserId { get; set; }

        public virtual ApplicationUser User { get; set; }
        public virtual Question Question { get; set; }

        public int Vote { get; set; } = 0;
        public bool IsCorrect { get; set; } = false;
        public virtual ICollection<Vote> Votes { get; set; }
        public virtual ICollection<AnswerComment> AnswerComments { get; set; }

        public Answer() { 
            Votes = new HashSet<Vote>();
            AnswerComments = new HashSet<AnswerComment>();
        }

        public Answer(string body, Question question, ApplicationUser user) { 
            Body = body;
            Question = question;
            QuestionId = question.Id;
            User = user;
            UserId = user.Id;
            Date = DateTime.Now;
            Votes = new HashSet<Vote>();
            AnswerComments = new HashSet<AnswerComment>();
        }
    }
}
