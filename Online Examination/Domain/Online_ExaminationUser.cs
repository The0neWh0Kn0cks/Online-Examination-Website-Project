using Microsoft.AspNetCore.Identity;

namespace Online_Examination.Domain
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class Online_ExaminationUser : IdentityUser
    {
        public virtual ICollection<Exam> CreatedExams { get; set; } = new List<Exam>();

        public virtual ICollection<Attempt> Attempts { get; set; } = new List<Attempt>();

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime ModifiedDate { get; set; } = DateTime.Now;

        public string Role { get; set; } = "Student";
    }
}