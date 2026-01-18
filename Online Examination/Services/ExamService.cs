using Microsoft.EntityFrameworkCore;
using Online_Examination.Data;
using Online_Examination.Domain;

namespace Online_Examination.Services
{
    public class ExamService
    {
        private readonly Online_ExaminationContext _context;

        public ExamService(Online_ExaminationContext context)
        {
            _context = context;
        }

        // ==========================================
        // 1. Admin Operations: Create & Update
        // ==========================================

        public async Task<Exam> AddExamAsync(Exam exam)
        {
            if (string.IsNullOrWhiteSpace(exam.AccessCode))
            {
                exam.AccessCode = GenerateAccessCode();
            }

            await EnsureUniqueAccessCodeAsync(exam.AccessCode);

            _context.Exams.Add(exam);
            await _context.SaveChangesAsync();

            return exam;
        }

        public async Task<Exam> UpdateExamAsync(Exam exam)
        {
            var existingExam = await _context.Exams
                .Include(e => e.Questions)
                .FirstOrDefaultAsync(e => e.Id == exam.Id);

            if (existingExam == null)
            {
                throw new Exception($"Exam with ID {exam.Id} not found.");
            }

            existingExam.Title = exam.Title;
            existingExam.Description = exam.Description;
            existingExam.TimeLimitMinutes = exam.TimeLimitMinutes;
            existingExam.IsPublished = exam.IsPublished;
            existingExam.Level = exam.Level;

            if (!string.IsNullOrWhiteSpace(exam.AccessCode) && exam.AccessCode != existingExam.AccessCode)
            {
                await EnsureUniqueAccessCodeAsync(exam.AccessCode, exam.Id);
                existingExam.AccessCode = exam.AccessCode;
            }

            _context.Exams.Update(existingExam);
            await _context.SaveChangesAsync();

            return existingExam;
        }

        public async Task DeleteExamAsync(int examId)
        {
            var exam = await _context.Exams.FindAsync(examId);
            if (exam == null)
            {
                throw new Exception($"Exam with ID {examId} not found.");
            }

            _context.Exams.Remove(exam);
            await _context.SaveChangesAsync();
        }

        // ==========================================
        // 2. Query Operations
        // ==========================================

        public async Task<List<Exam>> GetAllExamsAsync()
        {
            return await _context.Exams
                .Include(e => e.Creator)
                .Include(e => e.Questions)
                .Include(e => e.Attempts)
                .OrderByDescending(e => e.DateCreated)
                .ToListAsync();
        }

        public async Task<Exam?> GetExamByIdAsync(int examId)
        {
            return await _context.Exams
                .Include(e => e.Questions)
                .Include(e => e.Creator)
                .FirstOrDefaultAsync(e => e.Id == examId);
        }

        public async Task<Exam?> GetExamForEditAsync(int examId)
        {
            return await _context.Exams
                .Include(e => e.Questions)
                .Include(e => e.Creator)
                .FirstOrDefaultAsync(e => e.Id == examId);
        }

        public async Task<Exam?> GetExamByAccessCodeAsync(string accessCode)
        {
            if (string.IsNullOrWhiteSpace(accessCode))
            {
                return null;
            }

            return await _context.Exams
                .Include(e => e.Questions)
                .Include(e => e.Creator)
                .FirstOrDefaultAsync(e => e.AccessCode == accessCode && e.IsPublished);
        }

        // ==========================================
        // 3. Question Management
        // ==========================================

        public async Task AddQuestionAsync(Question question)
        {
            _context.Questions.Add(question);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateQuestionAsync(Question question)
        {
            var existingQuestion = await _context.Questions.FindAsync(question.Id);
            if (existingQuestion == null)
            {
                throw new Exception($"Question with ID {question.Id} not found.");
            }

            existingQuestion.Text = question.Text;
            existingQuestion.OptionA = question.OptionA;
            existingQuestion.OptionB = question.OptionB;
            existingQuestion.OptionC = question.OptionC;
            existingQuestion.OptionD = question.OptionD;
            existingQuestion.CorrectAnswer = question.CorrectAnswer;
            existingQuestion.ImageUrl = question.ImageUrl;
            existingQuestion.ReadingPassage = question.ReadingPassage;

            _context.Questions.Update(existingQuestion);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteQuestionAsync(int questionId)
        {
            var question = await _context.Questions.FindAsync(questionId);
            if (question == null)
            {
                throw new Exception($"Question with ID {questionId} not found.");
            }

            _context.Questions.Remove(question);
            await _context.SaveChangesAsync();
        }

        // ==========================================
        // 4. Helper Methods
        // ==========================================

        public string GenerateAccessCode()
        {
            return Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();
        }

        private async Task EnsureUniqueAccessCodeAsync(string accessCode, int? excludeExamId = null)
        {
            var query = _context.Exams.Where(e => e.AccessCode == accessCode);

            if (excludeExamId.HasValue)
            {
                query = query.Where(e => e.Id != excludeExamId.Value);
            }

            var exists = await query.AnyAsync();
            if (exists)
            {
                throw new Exception($"Access code '{accessCode}' is already in use. Please generate a new one.");
            }
        }

        public async Task<bool> AccessCodeExistsAsync(string accessCode)
        {
            return await _context.Exams.AnyAsync(e => e.AccessCode == accessCode);
        }

        // ==========================================
        // 5. Student Exam Submission & Grading
        // ==========================================

        public async Task<Attempt> SubmitExamAttemptAsync(int examId, string userId, Dictionary<int, string> studentAnswers)
        {
            var exam = await _context.Exams
                .Include(e => e.Questions)
                .FirstOrDefaultAsync(e => e.Id == examId);

            if (exam == null)
            {
                throw new Exception($"Exam with ID {examId} not found.");
            }

            if (string.IsNullOrEmpty(userId))
            {
                throw new Exception("User ID is required to submit exam.");
            }

            int totalScore = 0;
            int totalQuestions = exam.Questions.Count;

            foreach (var question in exam.Questions)
            {
                if (studentAnswers.ContainsKey(question.Id))
                {
                    string studentAnswer = studentAnswers[question.Id];
                    string correctAnswer = question.CorrectAnswer;

                    if (string.Equals(studentAnswer, correctAnswer, StringComparison.OrdinalIgnoreCase))
                    {
                        totalScore++;
                    }
                }
            }

            var attempt = new Attempt
            {
                UserId = userId,
                ExamId = examId,
                Score = totalScore
            };

            _context.Attempts.Add(attempt);
            await _context.SaveChangesAsync();

            return attempt;
        }

        public async Task<Attempt?> GetAttemptByIdAsync(int attemptId)
        {
            return await _context.Attempts
                .Include(a => a.Exam)
                    .ThenInclude(e => e.Questions)
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.Id == attemptId);
        }

        public async Task<List<Attempt>> GetUserAttemptsAsync(string userId)
        {
            return await _context.Attempts
                .Include(a => a.Exam)
                    .ThenInclude(e => e.Questions)
                .Include(a => a.User)
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.DateCreated)
                .ToListAsync();
        }
    }
}
