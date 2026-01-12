using System.ComponentModel.DataAnnotations;

namespace Online_Examination.Domain
{
    public class Question : BaseDomainModel // <--- 继承基类
    {
        [Required]
        public string Text { get; set; } = string.Empty;

        // --- 选项整合 ---
        [Required] public string OptionA { get; set; } = string.Empty;
        [Required] public string OptionB { get; set; } = string.Empty;
        [Required] public string OptionC { get; set; } = string.Empty;
        [Required] public string OptionD { get; set; } = string.Empty;

        [Required]
        public string CorrectAnswer { get; set; } = "A"; // 存储 "A", "B"...

        // --- Multimedia Support: For Math/Science questions with diagrams ---
        public string? ImageUrl { get; set; }

        // --- Reading Comprehension: For Language questions with passages ---
        public string? ReadingPassage { get; set; }

        // --- 外键：属于哪张卷子 ---
        public int ExamId { get; set; }
        public Exam? Exam { get; set; }
    }
}