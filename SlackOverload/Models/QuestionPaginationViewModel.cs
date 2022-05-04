namespace SlackOverload.Models {
    public class QuestionPaginationViewModel {
        public int currentPageIndex { get; set; }
        public int pageCount { get; set; }
        public List<Question> questionList { get; set; }
    }
}
