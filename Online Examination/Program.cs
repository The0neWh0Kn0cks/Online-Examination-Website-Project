using Domain;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Online_Examination.Components;
using Online_Examination.Components.Account;
using Online_Examination.Data;
// using Online_Examination.Domain; // 如果不再使用旧的 User 类，这行可能不需要了
using Online_Examination.Services;

var builder = WebApplication.CreateBuilder(args);

// ====================================================
// 1. 添加服务 (Add Services)
// ====================================================

// 添加 Blazor 服务
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// 添加 Controllers 支持 (API)
builder.Services.AddControllers();

// 配置 Authentication 状态级联
builder.Services.AddCascadingAuthenticationState();

// 配置 HttpClient
builder.Services.AddScoped(sp =>
{
    var navigationManager = sp.GetRequiredService<NavigationManager>();
    return new HttpClient
    {
        BaseAddress = new Uri(navigationManager.BaseUri)
    };
});

// --- 核心修改 A: 数据库上下文 ---
// 必须使用与 Identity 脚手架一致的 Context (Online_ExaminationContext)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<Online_ExaminationContext>(options =>
    options.UseSqlServer(connectionString));

// --- 核心修改 B: 错误修正后的 Identity 配置 ---
// 删除了冲突的 AddIdentity<User>，合并使用脚手架生成的配置
builder.Services.AddIdentityCore<Online_ExaminationUser>(options =>
{
    // 1. 登录要求 (从脚手架保留)
    options.SignIn.RequireConfirmedAccount = true;

    // 2. 密码强度设置 (从你旧代码迁移过来的宽松设置，方便测试)
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;

    // 3. 用户名/邮箱设置
    options.User.RequireUniqueEmail = true;
})
    .AddEntityFrameworkStores<Online_ExaminationContext>() // 确保这里指向新的 Context
    .AddSignInManager()
    .AddDefaultTokenProviders();

// 配置 Cookie 认证 (Blazor Identity 必需)
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
})
    .AddIdentityCookies();

// --- 核心修改 C: Identity 辅助服务 ---
// 这些是脚手架生成的辅助类，确保它们能正常工作
builder.Services.AddSingleton<IEmailSender<Online_ExaminationUser>, IdentityNoOpEmailSender>();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

// --- 业务服务 ---
builder.Services.AddScoped<StudentService>();
builder.Services.AddScoped<UserSession>();


// ====================================================
// 2. 构建应用 (Build App)
// ====================================================

var app = builder.Build();

// 配置 HTTP 请求管道
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

// 认证与授权中间件
app.UseAuthentication();
app.UseAuthorization();

// 映射 API Controllers
app.MapControllers();

// 映射 Blazor 组件
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// 映射 Identity 端点 (登录/注销等逻辑)
app.MapAdditionalIdentityEndpoints();

app.Run();