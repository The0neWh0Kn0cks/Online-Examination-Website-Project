using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Online_Examination.Domain; // 这里的 Exam, Attempt 实体类应该还在 Domain 里
using Online_Examination.Data;
using Domain;   // ✅ 必须添加：引用新的 Context 和 User 类

namespace Online_Examination.Services
{
    public class StudentService
    {
        // ✅ 修改 1: 替换为新的数据库上下文
        private readonly Online_ExaminationContext _context;

        // ✅ 修改 2: 替换为新的用户类 Online_ExaminationUser
        private readonly UserManager<Online_ExaminationUser> _userManager;
        private readonly SignInManager<Online_ExaminationUser> _signInManager;

        // 构造函数注入
        public StudentService(
            Online_ExaminationContext context, // ✅ 类型已更新
            UserManager<Online_ExaminationUser> userManager, // ✅ 类型已更新
            SignInManager<Online_ExaminationUser> signInManager) // ✅ 类型已更新
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // ==========================================
        // 1. 用户认证模块
        // ==========================================

        // 注册新学生
        // ✅ 参数类型改为 Online_ExaminationUser
        public async Task<Online_ExaminationUser> RegisterStudentAsync(Online_ExaminationUser user, string password)
        {
            // 检查邮箱是否已存在
            if (string.IsNullOrEmpty(user.Email))
            {
                throw new Exception("邮箱地址不能为空");
            }

            var existingUser = await _userManager.FindByEmailAsync(user.Email);
            if (existingUser != null)
            {
                throw new Exception("该邮箱已被注册");
            }

            // ⚠️ 注意：标准的 Online_ExaminationUser (继承自 IdentityUser) 通常没有 Role 属性。
            // 它的角色通常存在 AspNetUserRoles 表里。
            // 如果你的 Online_ExaminationUser 类里没有定义 public string Role { get; set; }，下面这行会报错。
            // 建议：先注释掉，后续通过 _userManager.AddToRoleAsync(user, "Student") 来添加角色。
            // user.Role = "Student"; 

            user.UserName = user.Email; // 使用邮箱作为用户名

            // 使用 UserManager 创建用户（自动哈希密码）
            var result = await _userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"注册失败: {errors}");
            }

            return user;
        }

        // 学生登录
        // ✅ 返回类型改为 Online_ExaminationUser
        public async Task<Online_ExaminationUser?> LoginAsync(string email, string password)
        {
            // 通过邮箱查找用户
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return null;
            }

            // 验证密码
            var result = await _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return user;
            }

            return null;
        }

        // ==========================================
        // 2. 考试查询模块
        // ==========================================

        // 获取所有可用的试卷
        public async Task<List<Exam>> GetAllExamsAsync()
        {
            return await _context.Exams.ToListAsync();
        }

        // 获取单张试卷的详细内容（开始考试用）
        public async Task<Exam?> GetExamByIdAsync(int examId)
        {
            return await _context.Exams
                .Include(e => e.Questions)
                .FirstOrDefaultAsync(e => e.Id == examId);
        }

        // ==========================================
        // 3. 核心业务：交卷 & 自动判分
        // ==========================================

        // ⚠️ 注意：这里的 userId 是 int。
        // 如果你的 Online_ExaminationUser 使用的是默认的 GUID (string)，这里可能需要改为 string userId。
        // 并且你需要检查 Attempt 实体里的 UserId 字段是 int 还是 string。
        public async Task<Attempt> SubmitExamAsync(int userId, int examId, Dictionary<int, string> studentAnswers)
        {
            var exam = await _context.Exams
                .Include(e => e.Questions)
                .FirstOrDefaultAsync(e => e.Id == examId);

            if (exam == null) throw new Exception("试卷不存在");

            int finalScore = 0;

            foreach (var question in exam.Questions)
            {
                if (studentAnswers.ContainsKey(question.Id))
                {
                    string myAnswer = studentAnswers[question.Id];

                    if (string.Equals(myAnswer, question.CorrectAnswer, StringComparison.OrdinalIgnoreCase))
                    {
                        finalScore++;
                    }
                }
            }

            var attempt = new Attempt
            {
                UserId = userId, // ⚠️ 确保 Attempt.UserId 的类型和这里的 userId 类型一致
                ExamId = examId,
                Score = finalScore,
                // DateCreated = DateTime.Now // 确保 Attempt 类里有这个字段，或者数据库会自动生成
            };

            _context.Attempts.Add(attempt);
            await _context.SaveChangesAsync();

            return attempt;
        }

        // ==========================================
        // 4. 历史记录模块
        // ==========================================

        public async Task<List<Attempt>> GetStudentHistoryAsync(int userId)
        {
            return await _context.Attempts
                .Where(a => a.UserId == userId)
                .Include(a => a.Exam)
                //.OrderByDescending(a => a.DateCreated) // 确保 Attempt 有 DateCreated 字段，否则会报错
                .ToListAsync();
        }
    }
}