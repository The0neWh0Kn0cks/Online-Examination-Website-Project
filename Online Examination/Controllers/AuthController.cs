using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Online_Examination.Domain; // 使用 Domain 命名空间

namespace Online_Examination.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        // 使用 Domain.User 而不是 ApplicationUser
        private readonly UserManager<Online_ExaminationUser> _userManager;

        public AuthController(UserManager<Online_ExaminationUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto model)
        {
            if (string.IsNullOrEmpty(model.Email))
            {
                return BadRequest("请输入邮箱地址。");
            }

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
                return NotFound("该邮箱未注册。");

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            return Ok(new
            {
                Message = "验证码已生成（开发模式直接返回）",
                Email = model.Email,
                Token = token
            });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model)
        {
            if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Token) || string.IsNullOrEmpty(model.NewPassword))
            {
                return BadRequest("信息不完整，请提供邮箱、验证码和新密码。");
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null) return BadRequest("用户不存在");

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);

            if (result.Succeeded)
            {
                return Ok("密码重置成功，请使用新密码登录。");
            }

            return BadRequest(result.Errors);
        }
    }

    public class ForgotPasswordDto
    {
        public string? Email { get; set; }
    }

    public class ResetPasswordDto
    {
        public string? Email { get; set; }
        public string? Token { get; set; }
        public string? NewPassword { get; set; }
    }
}