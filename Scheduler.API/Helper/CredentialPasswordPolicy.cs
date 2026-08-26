namespace Scheduler.API.Helper
{
    public static class CredentialPasswordPolicy
    {
        public const string RequirementMessage =
            "Password must be at least 6 characters and include one uppercase letter, one number, and one special character.";

        public static bool IsSatisfied(string password)
        {
            if (string.IsNullOrEmpty(password) || password.Length < 6)
                return false;

            var hasUpper = false;
            var hasDigit = false;
            var hasSpecial = false;
            foreach (var c in password)
            {
                if (char.IsUpper(c)) hasUpper = true;
                else if (char.IsDigit(c)) hasDigit = true;
                else if (!char.IsLetterOrDigit(c)) hasSpecial = true;
            }

            return hasUpper && hasDigit && hasSpecial;
        }
    }
}
