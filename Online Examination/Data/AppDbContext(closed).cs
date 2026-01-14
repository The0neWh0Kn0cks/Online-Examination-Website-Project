/*
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Online_Examination.Domain;
using System.Reflection;

namespace Online_Examination.Data
{
    /// <summary>
    /// 应用程序数据库上下文
    /// 继承自 IdentityDbContext 以支持 ASP.NET Core Identity
    /// </summary>
    public class AppDbContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // ==========================================
        // 声明业务表
        // ==========================================
        // 注意：Users 表已经由 IdentityDbContext 提供，不需要再次声明

        public DbSet<Exam> Exams { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Attempt> Attempts { get; set; }

        // ==========================================
        // 自动审计功能
        // ==========================================
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // 处理继承自 BaseDomainModel 的实体
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

            // 处理 User 表的审计字段（因为 User 继承 IdentityUser，不继承 BaseDomainModel）
            var userEntries = ChangeTracker.Entries<User>();

            foreach (var entry in userEntries)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedDate = DateTime.Now;
                    entry.Entity.ModifiedDate = DateTime.Now;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.ModifiedDate = DateTime.Now;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }

        // ==========================================
        // 配置表关系
        // ==========================================
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ⚠️ 重要：必须先调用 base.OnModelCreating()
            // 这会配置所有 Identity 相关的表（AspNetUsers, AspNetRoles 等）
            base.OnModelCreating(modelBuilder);

            // ✨ 自动应用所有 IEntityTypeConfiguration
            // 这会扫描 Data/Configurations 文件夹下的所有配置类
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            // ==========================================
            // 配置 User 实体
            // ==========================================
            modelBuilder.Entity<User>(entity =>
            {
                // 配置表名（如果想自定义表名，取消注释）
                // entity.ToTable("Users");

                // 确保 Email 唯一
                entity.HasIndex(u => u.Email).IsUnique();

                // 配置审计字段
                entity.Property(u => u.CreatedDate)
                    .IsRequired()
                    .HasDefaultValueSql("GETDATE()"); // SQL Server

                entity.Property(u => u.ModifiedDate)
                    .IsRequired()
                    .HasDefaultValueSql("GETDATE()");

                // 配置 Role 字段
                entity.Property(u => u.Role)
                    .IsRequired()
                    .HasMaxLength(20)
                    .HasDefaultValue("Student");

                // 配置导航属性
                entity.HasMany(u => u.CreatedExams)
                    .WithOne(e => e.Creator)
                    .HasForeignKey(e => e.CreatorId)
                    .OnDelete(DeleteBehavior.Restrict); // 防止级联删除

                entity.HasMany(u => u.Attempts)
                    .WithOne(a => a.User)
                    .HasForeignKey(a => a.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ==========================================
            // 配置 Identity 表名（可选）
            // ==========================================
            // 如果你想使用更简洁的表名，可以取消下面的注释
            
            modelBuilder.Entity<IdentityRole<int>>().ToTable("Roles");
            modelBuilder.Entity<IdentityUserRole<int>>().ToTable("UserRoles");
            modelBuilder.Entity<IdentityUserClaim<int>>().ToTable("UserClaims");
            modelBuilder.Entity<IdentityUserLogin<int>>().ToTable("UserLogins");
            modelBuilder.Entity<IdentityUserToken<int>>().ToTable("UserTokens");
            modelBuilder.Entity<IdentityRoleClaim<int>>().ToTable("RoleClaims");
            
        }
    }
}

*/