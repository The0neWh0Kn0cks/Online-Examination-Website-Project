using System.ComponentModel.DataAnnotations;

namespace Online_Examination.Domain
{
    public class Exam : BaseDomainModel
    {
        [Required]
        [MaxLength(100)]
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [Range(1, 180)]
        public int TimeLimitMinutes { get; set; } = 60;

        // --- 外键：老师 ---
        public int CreatorId { get; set; }
        public User? Creator { get; set; }

        // --- 导航属性 ---
        public List<Question> Questions { get; set; } = new List<Question>();
        public List<Attempt> Attempts { get; set; } = new List<Attempt>();
    }
}