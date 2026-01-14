using Microsoft.AspNetCore.Identity;

namespace Online_Examination.Domain
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class Online_ExaminationUser : IdentityUser
    {
        // ==========================================
        // 1. 必须添加：导航属性
        // 解决了 Context 文件里找不到 CreatedExams 和 Attempts 的报错
        // ==========================================

        // 用户创建的试卷列表
        public virtual ICollection<Exam> CreatedExams { get; set; } = new List<Exam>();

        // 用户的考试记录列表
        public virtual ICollection<Attempt> Attempts { get; set; } = new List<Attempt>();

        // ==========================================
        // 2. 建议添加：审计字段
        // 用于记录用户何时注册或修改信息
        // ==========================================
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime ModifiedDate { get; set; } = DateTime.Now;
    }
}