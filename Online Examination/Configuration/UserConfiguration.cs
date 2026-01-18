using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Online_Examination.Domain;

namespace Online_Examination.Data.Configurations
{

    public class UserConfiguration : IEntityTypeConfiguration<Online_ExaminationUser>
    {
        public void Configure(EntityTypeBuilder<Online_ExaminationUser> builder)
        {

            builder.Property(u => u.Role)
                .HasMaxLength(20)
                .HasDefaultValue("Student");
        }
    }
}