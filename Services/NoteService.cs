using NotesV1.Models.AttributeModel;
using NotesV1.Models.NoteModel;
using NotesV1.Models.Token;
using System.Data.SqlClient;

namespace NotesV1.Services
{
    public class NoteService
    {
        Note note = new Note();
        private readonly string connectionString = "Data Source=BEUDYFUL;Initial Catalog=notes;Integrated Security=True";
        private Token _token;
        private LoginService _loginService;
        public NoteService(LoginService loginService, Token token)
        {
            _token = token;
            _loginService = loginService;
        }

        /// <summary>
        /// Add notes to a project with an attribute
        /// in the attribute section, add multiple attributes using a comma
        /// no spaces after the comma for instance Priority,Low Budget
        /// </summary>
        /// <param name="noteText"></param>
        /// <param name="projectName"></param>
        /// <param name="attributeName"></param>
        /// <returns></returns>
        public bool AddNote(string noteText, string projectName, string attributeName)
        {
            bool result = false;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string sqlStm = "SELECT TOP 1 * FROM dbo.Note ORDER BY NoteId DESC";
                    using (SqlCommand command = new SqlCommand(sqlStm, connection))
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {

                                note.NoteId = reader.GetInt32(0);
                            }
                        }
                    }
                    connection.Close();
                }
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    String sql = "INSERT INTO dbo.Note (NoteId, NoteText, ProjectName, AttributeName) VALUES (@noteId, @noteText, @projectName, @attributeName) ";
                    using (SqlCommand command = new SqlCommand(sql, conn))
                    {
                        conn.Open();
                        string seperate = attributeName;
                        string[] values = seperate.Split(',');
                        for (int i = 0; i < values.Length; i++)
                        {
                            string value = values[i];
                            command.Parameters.Add("@noteId", System.Data.SqlDbType.VarChar, 255).Value = note.NoteId + 1;
                            command.Parameters.Add("@noteText", System.Data.SqlDbType.VarChar, 255).Value = noteText;
                            command.Parameters.Add("@projectName", System.Data.SqlDbType.VarChar, 255).Value = projectName;
                            command.Parameters.Add("@attributeName", System.Data.SqlDbType.VarChar, 255).Value = value;
                            command.ExecuteNonQuery();
                            command.Parameters.Clear();

                        }
                        //command.CommandType = System.Data.CommandType.StoredProcedure;

                    }
                }
                result = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("Unable to connect");

            }
            return result;

        }

        /// <summary>
        /// Update a note when the user is logged in
        /// </summary>
        /// <param name="noteId"></param>
        /// <param name="noteText"></param>
        /// <returns></returns>
        public bool UpdateNote(int noteId, string noteText)
        {
            bool result = false;
            int count = 0;
            if(_token.TempToken == null)
            {
                result =  false;
            }
            else
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        String sql = "UPDATE Note SET NoteText = @noteText WHERE NoteId = @noteId";
                        using (SqlCommand command = new SqlCommand(sql, conn))
                        {
                            conn.Open();

                            command.Parameters.Add("@noteId", System.Data.SqlDbType.VarChar, 255).Value = noteId;
                            command.Parameters.Add("@noteText", System.Data.SqlDbType.VarChar, 255).Value = noteText;
                            command.ExecuteNonQuery();
                            count = command.ExecuteNonQuery();
                        }
                        if (count > 0)
                        {
                            result = true;
                        }
                        //command.CommandType = System.Data.CommandType.StoredProcedure;

                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine("Unable to connect");

                }
            }
           

                return result;
        }

        /// <summary>
        /// Delete Note when the user is logged in
        /// </summary>
        /// <param name="noteId"></param>
        /// <returns></returns>
        public bool DeleteNote(int noteId)
        {
            int count = 0;
            bool result = false;
            if (_token.TempToken == null)
            {
                result = false;
            }
            else
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        String sql = "DELETE FROM dbo.Note WHERE NoteId = @noteId";
                        using (SqlCommand command = new SqlCommand(sql, conn))
                        {
                            conn.Open();

                            command.Parameters.Add("@noteId", System.Data.SqlDbType.VarChar, 255).Value = noteId;
                            command.ExecuteNonQuery();
                        }

                            result = true;

                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine("Unable to connect");

                }
            }

            return result;
        }

        /// <summary>
        /// Get all the notes uer and input a project id or attributeid
        /// user can also leave information blank and get results
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="attributeId"></param>
        /// <returns></returns>
        public List<Note> GetNotes(int? projectId, int? attributeId)
        {
            
            List<Note> notes = new List<Note>();

            if (_token.TempToken == null)
            {
                return null;
            }
            else if (attributeId == null && projectId == null)
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        String sql = "SELECT NoteId, NoteText, ProjectName, STUFF((SELECT DISTINCT ',' + AttributeName FROM Note n2 WHERE N2.NoteId = N1.NoteId  FOR XML PATH('')),1,1,'') AS AttributeName FROM Note n1 GROUP BY NoteId, NoteText, ProjectName";


                        using (SqlCommand command = new SqlCommand(sql, conn))
                        {
                            conn.Open();
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    Note newNote = new Note();
                                    List<DataAttribute> attributes = new List<DataAttribute>();
                                    
                                    newNote.NoteId = reader.GetInt32(0);
                                    newNote.NoteText = reader.GetString(1);
                                    newNote.ProjectName = reader.GetString(2);

                                    string seperate = reader.GetString(3);
                                    string[] values = seperate.Split(',');
                                    for (int i = 0; i < values.Length; i++)
                                    {
                                        string value = values[i];
                                        DataAttribute dataAttribute = new DataAttribute();
                                        dataAttribute.AttributeName = value;
                                        attributes.Add(dataAttribute);
                                    }

                                    newNote.Attributes = attributes;
                                    notes.Add(newNote);
                                }
                            }
                        }
                        conn.Close();

                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine("Unable to connect");

                }
            }
            else if (attributeId != null && projectId == null)
            {
                try
                {
                    
                    
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        String sql = "SELECT Note.NoteId, Note.AttributeName, Note.NoteText, Note.ProjectName, Note.Creation, Attribute.AttributeId" +
                        " FROM Note INNER JOIN Attribute on Note.AttributeName = Attribute.AttributeName WHERE Attribute.AttributeId = @attributeId";


                        using (SqlCommand command = new SqlCommand(sql, conn))
                        {
                            conn.Open();

                            command.Parameters.Add("@attributeId", System.Data.SqlDbType.Int, 40).Value = attributeId; ;
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    Note note = new Note();
                                    List<DataAttribute> attributes = new List<DataAttribute>();
                                    DataAttribute dataAttribute = new DataAttribute();
                                    note.NoteId = reader.GetInt32(0);
                                    dataAttribute.AttributeName = reader.GetString(1);
                                    attributes.Add(dataAttribute);
                                    note.Attributes = attributes;
                                    note.NoteText = reader.GetString(2);
                                    note.ProjectName = reader.GetString(3);
                                    note.Creation = reader.GetDateTime(4);
                                    notes.Add(note);
                                }
                            }

                        }

                        conn.Close();
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine("Unable to connect");

                }
            }

            else if (attributeId == null && projectId != null)
            {
                try
                {
                    
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        String sql = "SELECT DISTINCT a.NoteId, a.ProjectName, a.NoteText FROM note a" +
                                        " inner join Project b on a.ProjectName = b.ProjectName WHERE b.ProjectId = @projectId" +
                                            " Order by a.NoteId";


                        using (SqlCommand command = new SqlCommand(sql, conn))
                        {
                            conn.Open();

                            command.Parameters.Add("@projectId", System.Data.SqlDbType.VarChar, 40).Value = projectId;
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while(reader.Read())
                                {
                                    Note note = new Note();
                                    note.NoteId = reader.GetInt32(0);
                                    note.ProjectName = reader.GetString(1);
                                    note.NoteText = reader.GetString(2);
                                    notes.Add(note);
                                }
                            }

                        }
                        conn.Close();

                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine("Unable to connect");

                }
            }
            else if(attributeId != null && projectId != null)
            {
                try
                {

                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        String sql = "SELECT DISTINCT NoteId, NoteText, note.ProjectName, note.AttributeName, Attribute.AttributeId, Project.ProjectId" +
                                    " FROM Note inner JOIN Project on Note.ProjectName = Project.ProjectName" +
                                     " inner JOIN Attribute on Note.AttributeName = Attribute.AttributeName WHERE Project.ProjectId = @projectId and Attribute.AttributeId = @attributeId";


                        using (SqlCommand command = new SqlCommand(sql, conn))
                        {
                            conn.Open();

                            command.Parameters.Add("@projectId", System.Data.SqlDbType.VarChar, 40).Value = projectId;
                            command.Parameters.Add("@attributeId", System.Data.SqlDbType.VarChar, 40).Value = attributeId;
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    List<DataAttribute> attributes = new List<DataAttribute>();
                                    DataAttribute dataAttribute = new DataAttribute();                                   
                                    Note note = new Note();
                                    note.NoteId = reader.GetInt32(0);
                                    note.NoteText = reader.GetString(1);
                                    note.ProjectName = reader.GetString(2);
                                    dataAttribute.AttributeName = reader.GetString(3);
                                    attributes.Add(dataAttribute);
                                    note.Attributes = attributes;
                                    notes.Add(note);
                                }
                            }

                        }
                        conn.Close();

                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine("Unable to connect");

                }
            }
            return notes;
        }

        /// <summary>
        /// Return the count of all notes that have projects assigned to them and the ones that don't
        /// </summary>
        /// <returns></returns>
        public string GetProjectNoteCounts()
        {
            string result = "";
            int firstNum = 0;
            int secondNum = 0;
            if(_token.TempToken != null)
            {
                try
                {

                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        String sql = "SELECT COUNT( Distinct NoteId) FROM Note WHERE ProjectName is not null";


                        using (SqlCommand command = new SqlCommand(sql, conn))
                        {
                            conn.Open();

                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    firstNum = reader.GetInt32(0);

                                }
                            }

                        }
                        conn.Close();

                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine("Unable to connect");

                }
                try
                {

                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        String sql = "SELECT COUNT( Distinct NoteId) FROM Note WHERE ProjectName is null";


                        using (SqlCommand command = new SqlCommand(sql, conn))
                        {
                            conn.Open();

                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    secondNum = reader.GetInt32(0);

                                }
                            }

                        }
                        conn.Close();

                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine("Unable to connect");

                }

                result = "There are " + firstNum + " notes with projects and " + secondNum + " notes without projects";
            }
            else
            {
                result = "Error getting projects.";
            }
            
            return result;
        }


        /// <summary>
        /// Return the count of all notes that have attributes and the ones that don't
        /// </summary>
        /// <returns></returns>
        public string GetAttributesNoteCounts()
        {
            string result = "";
            int firstNum = 0;
            int secondNum = 0;
            if(_token.TempToken != null)
            {
                try
                {

                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        String sql = "SELECT COUNT( Distinct NoteId) FROM Note WHERE AttributeName is not null";


                        using (SqlCommand command = new SqlCommand(sql, conn))
                        {
                            conn.Open();

                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    firstNum = reader.GetInt32(0);

                                }
                            }

                        }
                        conn.Close();

                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine("Unable to connect");

                }
                try
                {

                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        String sql = "SELECT COUNT( Distinct NoteId) FROM Note WHERE AttributeName is null";


                        using (SqlCommand command = new SqlCommand(sql, conn))
                        {
                            conn.Open();

                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    secondNum = reader.GetInt32(0);

                                }
                            }

                        }
                        conn.Close();

                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine("Unable to connect");

                }

                result = "There are " + firstNum + " notes with attributes and " + secondNum + " notes without attributes";
                
            }
            else
            {
                result = "Error occured getting attributes.";
            }
            return result;
        }
    }
}
