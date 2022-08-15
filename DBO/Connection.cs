using NotesV1.Models;
using System.Data.SqlClient;

namespace NotesV1.DBO
{
    public class Connection
    {
        public List<User> users = new List<User>();
        private readonly string connectionString = "Data Source=BEUDYFUL;Initial Catalog=notes;Integrated Security=True";
        private User _user;

        public Connection(User user)
        {
            _user = user;
        }

        public List<User> GetAll()
        {


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM Users";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                _user.Id = reader.GetInt32(0);
                                _user.UserName = reader.GetString(1);
                                _user.Password = reader.GetString(2);
                                _user.Creation = reader.GetDateTime(3);
                                DateTime toString = reader.GetDateTime(4);

                                users.Add(_user);

                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.ToString());
            }
            return users;
        }

        public bool Login(String username, String password)
        {
            User user = new User();
            bool success = false;
            // var login = _conn.Login(username, password);
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {

                    String sql = "SELECT * FROM Users WHERE UserName = @username && Password = @password";
                    using (SqlCommand command = new SqlCommand(sql, conn))
                    {
                        //command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.Add(username, System.Data.SqlDbType.VarChar, 40).Value = user.UserName;
                        command.Parameters.Add(password, System.Data.SqlDbType.VarChar, 40).Value = user.Password;
                        conn.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                //_user.UserName = reader.GetString(1);
                                // _user.Password = reader.GetString(2);
                                success = true;
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

            return success;
        }
    }
}
