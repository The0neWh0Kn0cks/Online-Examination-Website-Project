namespace OnlineExamination.Domain
{
    public class ExamQuestion : BaseDomainModel
    {
        public int QuestionId { get; set; }

        public int ExamId { get; set; }


        // --- 导航属性 (Navigation Properties) ---
        // 这一步非常重要！Entity Framework (EF Core) 需要这些属性来帮你自动“连表查询”。
        // 有了它们，你写代码时就可以直接用 examQuestion.Question.QuestionText 拿到题目文字。

        // 关联到具体的题目对象
        public virtual Question? Question { get; set; }

        // 关联到具体的试卷对象
        // 注意：如果你还没有创建 Exam.cs，这行代码会暂时报错红线，创建好 Exam.cs 后就会消失
        public virtual Exam? Exam { get; set; }
    }
}