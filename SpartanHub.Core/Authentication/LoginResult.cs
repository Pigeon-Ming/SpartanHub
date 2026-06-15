namespace SpartanHub.Core.Authentication
{
    public class LoginResult
    {
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
        public SpartanAuthSession Session { get; set; }

        public static LoginResult Success(SpartanAuthSession session)
        {
            return new LoginResult
            {
                IsSuccess = true,
                Session = session
            };
        }

        public static LoginResult Failed(string errorMessage)
        {
            return new LoginResult
            {
                IsSuccess = false,
                ErrorMessage = errorMessage
            };
        }
    }
}
