using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Online_Examination.Domain;

namespace Online_Examination.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            // 1. 约束配置
            builder.HasIndex(u => u.Email).IsUnique();
            builder.Property(u => u.Role).HasMaxLength(20).HasDefaultValue("Student");

            // 注意：IdentityUser 已经有 UserName, Email, PasswordHash 等字段
            // 不需要配置 Username 和 Password

            // 2. 种子数据
            // 注意：由于使用了 Identity，密码需要哈希化
            // 这里先提供基础数据，密码需要在迁移后通过代码或手动添加
            
            var hasher = new PasswordHasher<User>();

            var adminUser = new User
            {
                Id = 1,
                UserName = "admin",
                NormalizedUserName = "ADMIN",
                Email = "admin@school.com",
                NormalizedEmail = "ADMIN@SCHOOL.COM",
                EmailConfirmed = true,
                Role = "Admin",
                SecurityStamp = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                CreatedDate = DateTime.Parse("2024-01-01"),
                ModifiedDate = DateTime.Parse("2024-01-01"),
                CreatedBy = "System"
            };
            adminUser.PasswordHash = hasher.HashPassword(adminUser, "Admin@123");

            var studentUser = new User
            {
                Id = 2,
                UserName = "john",
                NormalizedUserName = "JOHN",
                Email = "student@school.com",
                NormalizedEmail = "STUDENT@SCHOOL.COM",
                EmailConfirmed = true,
                Role = "Student",
                SecurityStamp = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                CreatedDate = DateTime.Parse("2024-01-01"),
                ModifiedDate = DateTime.Parse("2024-01-01"),
                CreatedBy = "System"
            };
            studentUser.PasswordHash = hasher.HashPassword(studentUser, "Student@123");

            builder.HasData(adminUser, studentUser);
        }
    }
}