namespace SlackOverload.Models {
    public partial class QuestionTag {
        public int Id { get; set; }
        public virtual Question? Question { get; set; }
        public int? QuestionId { get; set; }
        public string Tag { get; set; }

        public QuestionTag() { }

        public QuestionTag(Question question, string tag) { 
            Question = question;
            QuestionId = question.Id;
            Tag = tag;
        }
    }
}
