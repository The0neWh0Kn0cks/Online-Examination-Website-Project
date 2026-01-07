namespace OnlineExamination.Domain
{
    // 继承 BaseDomainModel，自动获得 Id (对应 ERD 里的 PK AnswerId)
    public class Answer : BaseDomainModel
    {
        // --- 基础内容 ---
        // 对应 ERD 里的 "AnswerText" (答案内容)
        public string? AnswerText { get; set; }

        // --- 外键 (Foreign Keys) ---

        // 对应 ERD 里的 "FK1 QuestionId"
        public int QuestionId { get; set; }

        // 对应 ERD 里的 "FK2 ExamId"
        public int ExamId { get; set; }

        // --- 导航属性 (让代码能自动抓取关联数据) ---

        // 这是一个“对一”关系，指向具体的题目
        public virtual Question? Question { get; set; }

        // 这是一个“对一”关系，指向具体的考试
        public virtual Exam? Exam { get; set; }
    }
}