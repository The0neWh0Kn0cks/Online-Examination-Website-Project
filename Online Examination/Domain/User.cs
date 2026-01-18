using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Online_Examination.Domain
{
    // ?? 建议保持类名为 Online_ExaminationUser，除非你准备好了去修改 Context 和 Program.cs
    public class Online_ExaminationUser : IdentityUser
    {
        // ==========================================
        // 1. 导航属性 (用于关联 Exam 和 Attempt)
        // ==========================================

        // 用户创建的试卷 (老师/管理员)
        public virtual ICollection<Exam> CreatedExams { get; set; } = new List<Exam>();

        // 用户的考试记录 (学生)
        public virtual ICollection<Attempt> Attempts { get; set; } = new List<Attempt>();

        // ==========================================
        // 2. 审计字段
        // ==========================================
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime ModifiedDate { get; set; } = DateTime.Now;

        // ==========================================
        // 3. 冗余角色字段 (兼容性保留)
        // ==========================================
        // 注意：Identity 标准做法是使用 AspNetRoles 表。
        // 但为了兼容你现有的 Login.razor 逻辑 (user.Role == "Student")，我们保留这个字段。
        [MaxLength(20)]
        public string Role { get; set; } = "Student";
    }
}