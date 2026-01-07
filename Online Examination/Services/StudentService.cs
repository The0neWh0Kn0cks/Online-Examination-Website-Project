using Microsoft.EntityFrameworkCore; // 必须引用：为了使用 ToListAsync 等数据库异步方法
using OnlineExamination.Data;        // 必须引用：为了能找到 DbContext
using OnlineExamination.Domain;      // 必须引用：为了能找到 Student 类

namespace OnlineExamination.Services
{
    public class StudentService
    {
        // 1. 声明一个私有的数据库上下文变量
        private readonly OnlineExaminationDbContext _context;

        // 2. 构造函数 (Constructor)
        // 当程序创建这个服务时，会自动把配置好的数据库连接 (DbContext) 注入进来
        public StudentService(OnlineExaminationDbContext context)
        {
            _context = context;
        }

        // --- 下面是具体的业务方法 ---

        // A. 获取所有学生 (Read All)
        public async Task<List<Student>> GetAllStudentsAsync()
        {
            // 相当于 SQL: SELECT * FROM Students
            // 使用 ToListAsync() 是为了不卡住网页界面
            return await _context.Students.ToListAsync();
        }

        // B. 根据 ID 获取单个学生 (Read One)
        public async Task<Student?> GetStudentByIdAsync(int id)
        {
            // 相当于 SQL: SELECT * FROM Students WHERE Id = id
            return await _context.Students.FindAsync(id);
        }

        // C. 添加新学生 (Create)
        public async Task AddStudentAsync(Student student)
        {
            // 这一步只是把数据放进内存里的"待保存区"
            _context.Students.Add(student);

            // 这一步才是真正执行 SQL INSERT 语句
            await _context.SaveChangesAsync();
        }

        // D. 更新学生信息 (Update)
        public async Task UpdateStudentAsync(Student student)
        {
            _context.Students.Update(student);
            await _context.SaveChangesAsync();
        }

        // E. 删除学生 (Delete)
        public async Task DeleteStudentAsync(int id)
        {
            // 先要把人找出来，才能删
            var student = await _context.Students.FindAsync(id);
            if (student != null)
            {
                _context.Students.Remove(student);
                await _context.SaveChangesAsync();
            }
        }
    }
}