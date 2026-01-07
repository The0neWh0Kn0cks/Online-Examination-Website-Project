namespace OnlineExamination.Domain
{
    // 继承 BaseDomainModel，自动获得 Id (对应 ERD 里的 PK ExamId)
    public class Exam : BaseDomainModel
    {

        // 对应 ERD 里的 "ExamName" (例如：2024期末数学考试)
        public string? ExamName { get; set; }
        public int QuestionId { get; set; }

        // 对应的导航属性 (让你能直接获取那个 Question 的详细信息)
        // 注意：因为 ExamQuestion 表已经处理了题目列表，这个字段在实际逻辑中可能很少用
        public virtual Question? Question { get; set; }

        // --- 关系列表 (最重要的部分) ---
        // 依据 ERD 的连线：Exam "Has" ExamQuestion, Exam "Has" ExamResult

        // 1. 一张试卷包含多道题目 (通过中间表 ExamQuestion 连接)
        // 对应 ERD 连向 ExamQuestion 的 "Has>" 线
        public virtual List<ExamQuestion>? ExamQuestions { get; set; }

        // 2. 一张试卷会有很多学生的成绩记录
        // 对应 ERD 连向 ExamResult 的 "Has>" 线
        // (需要等你创建 ExamResult.cs 后，这行代码才不会报错，暂时先写上)
        public virtual List<ExamResult>? ExamResults { get; set; }
    }
}