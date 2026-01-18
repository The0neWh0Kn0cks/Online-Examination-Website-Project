using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Online_Examination.Domain;
using Online_Examination.Data;

namespace Online_Examination.Services
{
    public class StudentService
    {
        private readonly Online_ExaminationContext _context;
        private readonly UserManager<Online_ExaminationUser> _userManager;
        private readonly SignInManager<Online_ExaminationUser> _signInManager;

        public StudentService(
            Online_ExaminationContext context,
            UserManager<Online_ExaminationUser> userManager,
            SignInManager<Online_ExaminationUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }


        public async Task<Online_ExaminationUser> RegisterStudentAsync(Online_ExaminationUser user, string password)
        {
            if (string.IsNullOrEmpty(user.Email))
            {
                throw new Exception("Email cannot be empty.");
            }

            var existingUser = await _userManager.FindByEmailAsync(user.Email);
            if (existingUser != null)
            {
                throw new Exception("Email had registerd.");
            }

            user.UserName = user.Email;

            var result = await _userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"Register to fail: {errors}");
            }

            return user;
        }

        public async Task<Microsoft.AspNetCore.Identity.SignInResult> LoginAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return Microsoft.AspNetCore.Identity.SignInResult.Failed;
            }

            return await _signInManager.PasswordSignInAsync(
                user, 
                password, 
                isPersistent: false, 
                lockoutOnFailure: false
            );
        }

        public async Task<Online_ExaminationUser?> GetUserByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        // ==========================================
        // 2. 考试查询模块
        // ==========================================

        public async Task<List<Exam>> GetAllExamsAsync()
        {
            return await _context.Exams.ToListAsync();
        }

        public async Task<Exam?> GetExamByIdAsync(int examId)
        {
            return await _context.Exams
                .Include(e => e.Questions)
                .FirstOrDefaultAsync(e => e.Id == examId);
        }

        // ==========================================
        // 3. 核心业务：交卷 & 自动判分
        // ==========================================

        public async Task<Attempt> SubmitExamAsync(string userId, int examId, Dictionary<int, string> studentAnswers)
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
                UserId = userId,
                ExamId = examId,
                Score = finalScore
            };

            _context.Attempts.Add(attempt);
            await _context.SaveChangesAsync();

            return attempt;
        }

        // ==========================================
        // 4. 历史记录模块
        // ==========================================

        public async Task<List<Attempt>> GetStudentHistoryAsync(string userId)
        {
            return await _context.Attempts
                .Where(a => a.UserId == userId)
                .Include(a => a.Exam)
                .ToListAsync();
        }
    }
}