namespace SlackOverload.Models {
    public partial class AnswerComment {
        public int Id { get; set; }
        public string Body { get; set; }
        public string UserId { get; set; }
        public int? AnswerId { get; set; }
        public DateTime Date { get; set; }

        public virtual ApplicationUser User { get; set; }
        public virtual Answer Answer { get; set; }
        public AnswerComment() { }
        public AnswerComment(string body, ApplicationUser user, Answer answer) {
            Body = body;
            User = user;
            UserId = user.Id;
            Answer = answer;
            AnswerId = answer.Id;
            Date = DateTime.Now;
        }
    }
}
