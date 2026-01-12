using System.ComponentModel.DataAnnotations;

namespace Online_Examination.Domain
{
    public class User : BaseDomainModel
    {
        [Required]
        [MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public string Role { get; set; } = "Student";

        // --- ÃÜÂëÕÒ»Ø×Ö¶Î ---
        public string? PasswordResetToken { get; set; }
        public DateTime? PasswordResetTokenExpiry { get; set; }

        // --- µ¼º½ÊôĞÔ ---
        public List<Exam> CreatedExams { get; set; } = new List<Exam>();
        public List<Attempt> Attempts { get; set; } = new List<Attempt>();
    }
}
