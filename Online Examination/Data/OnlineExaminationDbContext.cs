using Microsoft.EntityFrameworkCore;
using Online_Examination.Domain;

namespace Online_Examination.Data
{
    // 必须继承 DbContext
    public class AppDbContext : DbContext
    {
        // 1. 构造函数：接受配置选项 (连接字符串等)
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // 2. 声明表：告诉 EF Core 数据库里有哪些表
        public DbSet<User> Users { get; set; }
        public DbSet<Exam> Exams { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Attempt> Attempts { get; set; }

        // 3. 【高级功能】自动管理创建时间和更新时间
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // 找出所有继承了 BaseDomainModel 且状态为“添加”或“修改”的实体
            var entries = ChangeTracker.Entries<BaseDomainModel>();

            foreach (var entry in entries)
            {
                // 如果是新创建的数据
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.DateCreated = DateTime.Now;
                    entry.Entity.DateUpdated = DateTime.Now;
                }
                // 如果是修改过的数据
                else if (entry.State == EntityState.Modified)
                {
                    // 只更新修改时间，创建时间不动
                    entry.Entity.DateUpdated = DateTime.Now;
                }
            }

            // 继续执行原本的保存逻辑
            return base.SaveChangesAsync(cancellationToken);
        }

        // 4. (可选) 配置表关系的细节
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 举例：确保 User 的 Email 是唯一的，不能重复注册
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // === FIX: Resolve Multiple Cascade Paths Error for Attempts Table ===
            // Configure Attempt entity relationships to prevent cascade delete conflicts
            modelBuilder.Entity<Attempt>()
                .HasOne(a => a.User)
                .WithMany(u => u.Attempts)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Restrict);  // Prevent automatic deletion of attempts when user is deleted

            modelBuilder.Entity<Attempt>()
                .HasOne(a => a.Exam)
                .WithMany(e => e.Attempts)
                .HasForeignKey(a => a.ExamId)
                .OnDelete(DeleteBehavior.Restrict);  // Prevent automatic deletion of attempts when exam is deleted
        }
    }
}