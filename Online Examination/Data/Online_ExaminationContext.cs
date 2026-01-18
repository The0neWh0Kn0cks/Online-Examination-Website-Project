using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Online_Examination.Domain;
using System.Reflection;

namespace Online_Examination.Data
{
    public class Online_ExaminationContext : IdentityDbContext<Online_ExaminationUser>
    {
        public Online_ExaminationContext(DbContextOptions<Online_ExaminationContext> options)
            : base(options)
        {
        }
        public DbSet<Exam> Exams { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Attempt> Attempts { get; set; }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {

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


            var userEntries = ChangeTracker.Entries<Online_ExaminationUser>();
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
   
            base.OnModelCreating(modelBuilder);


            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());


            modelBuilder.Entity<Online_ExaminationUser>(entity =>
            {
       
                entity.HasMany(u => u.CreatedExams)
                    .WithOne(e => e.Creator)
                    .HasForeignKey(e => e.CreatorId)
                    .OnDelete(DeleteBehavior.Restrict);

        
                entity.HasMany(u => u.Attempts)
                    .WithOne(a => a.User)
                    .HasForeignKey(a => a.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}