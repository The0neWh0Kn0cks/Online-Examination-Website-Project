using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Online_Examination.Domain
{
    public class User : IdentityUser<int> // 使用 int 作为主键，保持与 BaseDomainModel 一致
    {
        // 注意：IdentityUser 已经包含了 Email, UserName 等字段，所以这里可以移除重复的

        // 角色字段
        [Required]
        [MaxLength(20)]
        public string Role { get; set; } = "Student"; // 默认为 Student

        // --- 忘记密码字段 ---
        // 注意：IdentityUser 已经有内置的密码重置机制，这些字段可以删除
        // public string? PasswordResetToken { get; set; }
        // public DateTime? PasswordResetTokenExpiry { get; set; }

        // --- 审计字段（从 BaseDomainModel 继承） ---
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public string? CreatedBy { get; set; }
        public DateTime ModifiedDate { get; set; } = DateTime.UtcNow;
        public string? ModifiedBy { get; set; }

        // --- 导航属性 (EF Core 关系映射) ---

        // 1. 如果这个用户是「老师/管理员」，他创建的所有考卷列表
        public List<Exam> CreatedExams { get; set; } = new List<Exam>();

        // 2. 如果这个用户是「学生」，他参加过的考试成绩记录
        public List<Attempt> Attempts { get; set; } = new List<Attempt>();
    }
}