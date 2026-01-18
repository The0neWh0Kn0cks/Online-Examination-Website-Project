using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Online_Examination.Domain;

namespace Online_Examination.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly SignInManager<Online_ExaminationUser> _signInManager;
        private readonly UserManager<Online_ExaminationUser> _userManager;

        public LoginController(
            SignInManager<Online_ExaminationUser> signInManager,
            UserManager<Online_ExaminationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpPost]
        [IgnoreAntiforgeryToken] // ? Critical: Disable token check for this endpoint
        public async Task<IActionResult> Login([FromForm] string email, [FromForm] string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                return BadRequest("Email and password are required.");
            }

            var result = await _signInManager.PasswordSignInAsync(
                email,
                password,
                isPersistent: false,
                lockoutOnFailure: false);

            if (result.Succeeded)
            {
                // Get user to determine role-based redirect
                var user = await _userManager.FindByEmailAsync(email);
                if (user != null)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    var role = roles.FirstOrDefault() ?? user.Role ?? "Student";

                    // Redirect based on role
                    if (role == "Admin")
                    {
                        return Redirect("/admin-dashboard");
                    }
                    else
                    {
                        return Redirect("/user-dashboard");
                    }
                }

                return Redirect("/");
            }

            return BadRequest("Invalid login attempt.");
        }
    }
}
