namespace Online_Examination.Services
{
    public class UserSession
    {
        // ✅ 修改 1: Identity 默认使用 String (GUID) 作为主键，而不是 int
        public string CurrentUserId { get; set; } = string.Empty;

        public string CurrentUserName { get; set; } = string.Empty;
        public string CurrentUserEmail { get; set; } = string.Empty;
        public string CurrentUserRole { get; set; } = string.Empty;

        // ✅ 修改 2: 判断登录状态改为检查 ID 字符串是否非空
        public bool IsLoggedIn => !string.IsNullOrEmpty(CurrentUserId);

        // ✅ 修改 3: SetUser 方法的参数 userId 类型改为 string
        public void SetUser(string userId, string userName, string email, string role)
        {
            CurrentUserId = userId;
            CurrentUserName = userName;
            CurrentUserEmail = email;
            CurrentUserRole = role;
        }

        public void Logout()
        {
            // ✅ 修改 4: 重置为空字符串
            CurrentUserId = string.Empty;
            CurrentUserName = string.Empty;
            CurrentUserEmail = string.Empty;
            CurrentUserRole = string.Empty;
        }
    }
}