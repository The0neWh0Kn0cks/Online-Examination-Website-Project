using Microsoft.AspNetCore.Identity;
using Online_Examination.Domain;

namespace Online_Examination.Data
{
    public static class DatabaseSeeder
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<Online_ExaminationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();


            string[] roleNames = { "Admin", "Student" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }


            var adminEmail = "admin@test.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                var newAdmin = new Online_ExaminationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true, 
                    Role = "Admin",       
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now
                };

       
                var result = await userManager.CreateAsync(newAdmin, "Admin123");

                if (result.Succeeded)
                {
            
                    await userManager.AddToRoleAsync(newAdmin, "Admin");
                }
            }

   
            var studentEmail = "student@test.com";
            var studentUser = await userManager.FindByEmailAsync(studentEmail);

            if (studentUser == null)
            {
                var newStudent = new Online_ExaminationUser
                {
                    UserName = studentEmail,
                    Email = studentEmail,
                    EmailConfirmed = true, 
                    Role = "Student",     
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now
                };

                var result = await userManager.CreateAsync(newStudent, "Student123");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(newStudent, "Student");
                }
            }
        }
    }
}