namespace SlackOverload.Models {
    public partial class Vote {
        public int Id { get; set; }
        public int VoteValue { get; set; }
        public string UserId { get; set; }
        public int? AnswerId { get; set; }
        public DateTime Date { get; set; }

        public virtual ApplicationUser User { get; set; }
        public virtual Answer Answer { get; set; }

        public Vote() { }

        public Vote(int voteValue, ApplicationUser user, Answer answer) {
            VoteValue = voteValue;
            User = user;
            UserId = user.Id;
            Answer = answer;
            AnswerId = answer.Id;
            Date = DateTime.Now;
        }
    }
}
