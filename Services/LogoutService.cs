using NotesV1.Models.Token;

namespace NotesV1.Services
{
    public class LogoutService
    {
        private readonly string connectionString = "Data Source=BEUDYFUL;Initial Catalog=notes;Integrated Security=True";
        private Token _token;
        private LoginService _loginService;
        public LogoutService(Token token)
        {
            _token = token;
        }

        public bool UserLogOut()
        {
            bool result = false;
            if (_token.TempToken == null)
            {
                result = false;
            }
            else
            {
                _token.TempToken = null;
                result = true;
            }
            return result;
        }
    }
}
