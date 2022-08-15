using Microsoft.IdentityModel.Tokens;
using NotesV1.DBO;
using NotesV1.Models;
using NotesV1.Models.Token;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace NotesV1.Services
{
    public class LoginService
    {
        User user = new User();
        private IConfiguration _configuration;
        private Token _token;
        private readonly string connectionString = "Data Source=BEUDYFUL;Initial Catalog=notes;Integrated Security=True";
        private readonly Connection _conn;
        Token tempToken = new Token();
        public LoginService(IConfiguration configuration, Token token)
        {
            _configuration = configuration;
            _token = token;
        }
        public bool VerifyUser(string username, string password)
        {
            bool verify = false;
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {

                    String sql = "SELECT * FROM Users WHERE UserName = @username AND Password = @password";
                    using (SqlCommand command = new SqlCommand(sql, conn))
                    {
                        command.Parameters.Add("@username", System.Data.SqlDbType.VarChar, 40).Value = username;
                        command.Parameters.Add("@password", System.Data.SqlDbType.VarChar, 40).Value = password;
                        conn.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                user.UserName = reader.GetString(1);
                                user.Password = reader.GetString(2);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("Unable to connect");
            }
            
            if(user.UserName == "" || user.Password == "")
            {
                verify = false;
            }
            else
            {
                verify = true;
            }
            return verify;
        }

        public string Token()
        {
            string username = user.UserName;
            string password = user.Password;
            var userVerify = VerifyUser(username, password);
            if (userVerify == false)
            {
                return null;
            }
            var key = _configuration.GetSection("AppSettings:Token").Value;
            var tokenKey = Encoding.ASCII.GetBytes(key);
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                    {
                    new Claim(ClaimTypes.Name, user.UserName)
                    }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(tokenKey),
                        SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            _token.TempToken = tokenHandler.WriteToken(token);
            return tokenHandler.WriteToken(token);
        }
    }
}
