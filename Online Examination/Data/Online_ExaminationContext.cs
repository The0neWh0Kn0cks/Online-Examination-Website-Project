using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Reflection;

namespace Online_Examination.Domain
{
    /// <summary>
    /// 应用程序数据库上下文
    /// 继承自 IdentityDbContext 以支持 ASP.NET Core Identity
    /// </summary>
    public class Online_ExaminationContext : IdentityDbContext<Online_ExaminationUser>
    {
        public Online_ExaminationContext(DbContextOptions<Online_ExaminationContext> options)
            : base(options)
        {
        }

        // ==========================================
        // 声明业务表 (DbSets)
        // ==========================================
        public DbSet<Exam> Exams { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Attempt> Attempts { get; set; }

        // ==========================================
        // 自动审计功能 (Audit)
        // ==========================================
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // 1. 处理继承自 BaseDomainModel 的实体 (如 Exam, Question 等)
            // 确保你的 Domain 实体确实继承了 BaseDomainModel
            var entries = ChangeTracker.Entries<BaseDomainModel>();

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.DateCreated = DateTime.Now;
                    entry.Entity.DateUpdated = DateTime.Now;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.DateUpdated = DateTime.Now;
                }
            }

            // 2. 处理 User 表的审计字段
            // ⚠️ 注意：标准的 Online_ExaminationUser (生成代码) 可能没有 CreatedDate/ModifiedDate 属性。
            // 如果你还没有在 Online_ExaminationUser.cs 中添加这些属性，下面这段代码会报错。
            // 建议：如果你需要记录用户的创建时间，请去 Online_ExaminationUser.cs 添加这两个属性。
            /* var userEntries = ChangeTracker.Entries<Online_ExaminationUser>();

            foreach (var entry in userEntries)
            {
                if (entry.State == EntityState.Added)
                {
                    // 确保 Online_ExaminationUser 类里有这些字段
                    // entry.Entity.CreatedDate = DateTime.Now; 
                    // entry.Entity.ModifiedDate = DateTime.Now;
                }
                else if (entry.State == EntityState.Modified)
                {
                    // entry.Entity.ModifiedDate = DateTime.Now;
                }
            }
            */

            return base.SaveChangesAsync(cancellationToken);
        }

        // ==========================================
        // 配置表关系 (Fluent API)
        // ==========================================
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ⚠️ 重要：必须先调用 base.OnModelCreating() 以配置 Identity 表
            base.OnModelCreating(modelBuilder);

            // 自动应用所有 IEntityTypeConfiguration
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            // ==========================================
            // 配置 Online_ExaminationUser 实体
            // ==========================================
            modelBuilder.Entity<Online_ExaminationUser>(entity =>
            {
                // 1. 配置审计字段 (如果你的 User 类里加了这些字段，请取消注释)
                /*
                entity.Property(u => u.CreatedDate)
                    .IsRequired()
                    .HasDefaultValueSql("GETDATE()");

                entity.Property(u => u.ModifiedDate)
                    .IsRequired()
                    .HasDefaultValueSql("GETDATE()");
                */

                // 2. 配置 Role 字段 (如果你的 User 类里加了这个字段)
                // ⚠️ Identity 默认使用 UserRoles 表，不建议在 User 表里直接存 Role 字符串。
                // 如果为了兼容旧代码保留了 public string Role {get;set;}，则保留下面配置。
                /*
                entity.Property(u => u.Role)
                    .HasMaxLength(20)
                    .HasDefaultValue("Student");
                */

                // 3. 配置导航属性 - ⚠️ 重点注意类型匹配

                // 配置 User -> CreatedExams
                entity.HasMany(u => u.CreatedExams)
                    .WithOne(e => e.Creator)
                    .HasForeignKey(e => e.CreatorId)
                    // ⚠️ 注意：Exam.CreatorId 的类型必须是 string (因为 Identity User ID 是 string)
                    .OnDelete(DeleteBehavior.Restrict);

                // 配置 User -> Attempts
                entity.HasMany(u => u.Attempts)
                    .WithOne(a => a.User)
                    .HasForeignKey(a => a.UserId)
                    // ⚠️ 注意：Attempt.UserId 的类型必须是 string
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}