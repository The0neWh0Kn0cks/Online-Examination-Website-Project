namespace OnlineExamination.Domain
{
    // 继承 BaseDomainModel，自动获得 Id (对应 ERD 里的 PK StudentExamId)
    // 注意：虽然 ERD 叫 StudentExamId，但在代码里我们统一用 Id 方便管理
    public class ExamResult : BaseDomainModel
    {
        // --- 核心数据 ---

        // 对应 ERD 里的 "Score" (这道题得了多少分)
        public int Score { get; set; }


        // --- 外键 (连接三个核心对象) ---

        // 1. 对应 ERD 里的 "FK2 UserId"
        // 既然我们之前创建的类叫 Student.cs，这里建议改名叫 StudentId 以保持一致
        // 如果数据库里非要叫 UserId，可以用 [ForeignKey("UserId")] 特性，但现在先保持简单
        public int StudentId { get; set; }

        // 2. 对应 ERD 里的 "FK3 ExamId" (哪场考试)
        public int ExamId { get; set; }

        // 3. 对应 ERD 里的 "FK1 QuestionId" (哪道题)
        public int QuestionId { get; set; }


        // --- 导航属性 (Navigation Properties) ---

        // 关联到学生 (注意这里类型是 Student，不是 User)
        public virtual Student? Student { get; set; }

        // 关联到考试
        public virtual Exam? Exam { get; set; }

        // 关联到题目
        public virtual Question? Question { get; set; }
    }
}