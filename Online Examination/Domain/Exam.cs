using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Online_Examination.Domain
{
    [Index(nameof(AccessCode), IsUnique = true)]
    public class Exam : BaseDomainModel
    {
        [Required]
        [MaxLength(100)]
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [Range(1, 180)]
        public int TimeLimitMinutes { get; set; } = 60;

        [Required]
        [MaxLength(20)]
        public string AccessCode { get; set; } = string.Empty;

        public bool IsPublished { get; set; } = false;

        // --- 外键：老师 ---
        public string CreatorId { get; set; }
        public Online_ExaminationUser? Creator { get; set; }

        // --- 导航属性 ---
        public List<Question> Questions { get; set; } = new List<Question>();
        public List<Attempt> Attempts { get; set; } = new List<Attempt>();
    }
}