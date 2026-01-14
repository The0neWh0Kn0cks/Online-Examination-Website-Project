using System.ComponentModel.DataAnnotations;

namespace Online_Examination.Domain
{
    public class Attempt : BaseDomainModel
    {
        public int Score { get; set; }

        // --- 外键：学生 ---

        //  1. 类型改对了，Identity 的 ID 是 string
        public string UserId { get; set; }

        //  2. 类型必须是新的 Online_ExaminationUser，不能是旧的 User
        public Online_ExaminationUser? User { get; set; }

        // --- 外键：卷子 ---
        public int ExamId { get; set; }
        public Exam? Exam { get; set; }
    }
}