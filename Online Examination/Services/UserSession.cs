namespace Online_Examination.Services
{
    public class UserSession
    {
        public string CurrentUserId { get; set; } = string.Empty;

        public string CurrentUserName { get; set; } = string.Empty;
        public string CurrentUserEmail { get; set; } = string.Empty;
        public string CurrentUserRole { get; set; } = string.Empty;

        public bool IsLoggedIn => !string.IsNullOrEmpty(CurrentUserId);

        public void SetUser(string userId, string userName, string email, string role)
        {
            CurrentUserId = userId;
            CurrentUserName = userName;
            CurrentUserEmail = email;
            CurrentUserRole = role;
        }

        public void Logout()
        {
            CurrentUserId = string.Empty;
            CurrentUserName = string.Empty;
            CurrentUserEmail = string.Empty;
            CurrentUserRole = string.Empty;
        }
    }
}