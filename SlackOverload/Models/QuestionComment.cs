namespace SlackOverload.Models {
    public partial class QuestionComment {
        public int Id { get; set; }
        public string Body { get; set; }
        public string UserId { get; set; }
        public int? QuestionId { get; set; }
        public DateTime Date { get; set; }

        public virtual ApplicationUser User { get; set; }
        public virtual Question Question { get; set; }

        public QuestionComment() { }
        public QuestionComment(string body, ApplicationUser user, Question question) {
            Body = body;
            User = user;
            UserId = user.Id;
            Question = question;
            QuestionId = question.Id;
            Date = DateTime.Now;
        }
    }
}
