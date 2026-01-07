namespace OnlineExamination.Domain 
{
    public class Question : BaseDomainModel
    {
        public string? QuestionText { get; set; }
        public int QuestionType { get; set; }
        public string? OptionA { get; set; }
        public string? OptionB { get; set; }
        public string? OptionC { get; set; }
        public string? OptionD { get; set; }
        public string? CorrectAnswer { get; set; }
        public int Score { get; set; }
    }
}