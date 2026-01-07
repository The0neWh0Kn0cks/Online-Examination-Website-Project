using Microsoft.EntityFrameworkCore;
using OnlineExamination.Domain;

namespace OnlineExamination.Data
{
    public class OnlineExaminationDbContext : DbContext
    {
        public OnlineExaminationDbContext(DbContextOptions<OnlineExaminationDbContext> options)
            : base(options)
        {
        }

        // --- 数据库表定义 ---
        public DbSet<Student> Students { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Exam> Exams { get; set; }
        public DbSet<ExamQuestion> ExamQuestions { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<ExamResult> ExamResults { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ============================================================
            // 🚫 核心修复：全面切断所有“多重级联路径”冲突
            // 将所有可能构成“三角形”关系的删除行为改为 Restrict (限制)
            // ============================================================

            // 1. 【修复刚才的报错】ExamQuestions (试卷-题目中间表)
            // 也就是：删题目时，如果题目还在试卷里，禁止删除（或者手动先移出试卷）
            modelBuilder.Entity<ExamQuestion>()
                .HasOne(eq => eq.Question)
                .WithMany()
                .HasForeignKey(eq => eq.QuestionId)
                .OnDelete(DeleteBehavior.Restrict); // <--- 关键修改

            modelBuilder.Entity<ExamQuestion>()
                .HasOne(eq => eq.Exam)
                .WithMany(e => e.ExamQuestions)
                .HasForeignKey(eq => eq.ExamId)
                .OnDelete(DeleteBehavior.Restrict);

            // 2. Exam 表本身也有一个 QuestionId (你ERD里的特殊设计)
            // 这也是导致冲突的根源之一：删题目 -> 删试卷 -> 删中间表 (形成了死循环)
            // 所以必须把这个也关掉：
            modelBuilder.Entity<Exam>()
                .HasOne(e => e.Question)
                .WithMany()
                .HasForeignKey(e => e.QuestionId)
                .OnDelete(DeleteBehavior.Restrict);

            // 3. 答案 (Answer) 表
            modelBuilder.Entity<Answer>()
                .HasOne(a => a.Question)
                .WithMany()
                .HasForeignKey(a => a.QuestionId)
                .OnDelete(DeleteBehavior.Restrict);

            // 4. 成绩 (ExamResult) 表 - 全部切断自动删除
            modelBuilder.Entity<ExamResult>()
                .HasOne(er => er.Exam)
                .WithMany(e => e.ExamResults)
                .HasForeignKey(er => er.ExamId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ExamResult>()
                .HasOne(er => er.Student)
                .WithMany()
                .HasForeignKey(er => er.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ExamResult>()
               .HasOne(er => er.Question)
               .WithMany()
               .HasForeignKey(er => er.QuestionId)
               .OnDelete(DeleteBehavior.Restrict);

            // ============================================================
            // 种子数据 (Seed Data)
            // ============================================================
            modelBuilder.Entity<Student>().HasData(
                new Student
                {
                    Id = 1,
                    Username = "Admin",
                    Password = "password123",
                    Email = "admin@school.com",
                    Role = "Admin",
                    DateCreated = DateTime.Now,
                    DateUpdated = DateTime.Now,
                    CreatedBy = "System",
                    UpdatedBy = "System"
                }
            );
        }
    }
}