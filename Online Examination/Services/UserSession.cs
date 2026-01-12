namespace Online_Examination.Services
{
    public class UserSession
    {
        public int CurrentUserId { get; set; }
        public string CurrentUserName { get; set; } = string.Empty;
        public string CurrentUserEmail { get; set; } = string.Empty;
        public string CurrentUserRole { get; set; } = string.Empty;

        public bool IsLoggedIn => CurrentUserId > 0;

        public void SetUser(int userId, string userName, string email, string role)
        {
            CurrentUserId = userId;
            CurrentUserName = userName;
            CurrentUserEmail = email;
            CurrentUserRole = role;
        }

        public void Logout()
        {
            CurrentUserId = 0;
            CurrentUserName = string.Empty;
            CurrentUserEmail = string.Empty;
            CurrentUserRole = string.Empty;
        }
    }
}
