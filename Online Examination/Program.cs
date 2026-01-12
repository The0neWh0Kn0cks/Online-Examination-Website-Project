using Microsoft.EntityFrameworkCore;
using Online_Examination.Components;
using Online_Examination.Data;
using Online_Examination.Services;

var builder = WebApplication.CreateBuilder(args);

// ====================================================
// 1. 服务注册区 (Add Services)
// ====================================================

// 添加 Blazor 服务 (支持服务端交互渲染)
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// --- 核心配置 A: 数据库连接 ---
// 读取 appsettings.json 里的 "DefaultConnection"
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));
// 注意: 如果你用的是 SQLite，这里要改成 options.UseSqlite(connectionString)

// --- 核心配置 B: 注册你的自定义服务 ---
// AddScoped 表示：每次 HTTP 请求创建一个新的实例 (最适合数据库操作的服务)
builder.Services.AddScoped<StudentService>();
builder.Services.AddScoped<UserSession>();

// ====================================================
// 2. 管道构建区 (Build App)
// ====================================================

var app = builder.Build();

// 配置 HTTP 请求管道
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // HSTS 值默认为 30 天，生产环境建议改大
    app.UseHsts();
}

app.UseHttpsRedirection();

// --- 核心配置 C: 静态文件 ---
// 这一行非常重要！没有它，你的 CSS/JS/图片 都加载不出来
app.UseStaticFiles();

app.UseAntiforgery();

// 映射 Blazor 组件
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode(); // 开启交互模式，否则按钮点击没反应

app.Run();