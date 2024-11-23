namespace Mentorshiptask.Model
{
    public class User
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public int FailedLoginAttempts { get; set; } = 0;
        public bool IsLocked { get; set; } = false;
    }

}
