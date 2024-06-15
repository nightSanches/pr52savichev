using MySql.Data.MySqlClient;
using pr52savichev.Classes.Common;
using pr52savichev.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pr52savichev.Classes
{
    public class StudentContext : Student
    {
        public StudentContext(int Id, string FirstName, string LastName, int IdGroup, bool Expelled, DateTime DateExpelled) :
            base(Id, FirstName, LastName, IdGroup, Expelled, DateExpelled)
        { }
        public static List<StudentContext> AllStudent()
        {
            List<StudentContext> allStudent = new List<StudentContext>();
            MySqlConnection connection = Connection.OpenConnection();
            MySqlDataReader BDStudents = Connection.Query("SELECT * FROM student ORDER BY Lastname", connection);
            while (BDStudents.Read())
            {
                allStudent.Add(new StudentContext(
                    BDStudents.GetInt32(0),
                    BDStudents.GetString(1),
                    BDStudents.GetString(2),
                    BDStudents.GetInt32(3),
                    BDStudents.GetBoolean(4),
                    BDStudents.IsDBNull(5) ? DateTime.Now : BDStudents.GetDateTime(5)
                    ));
            }
            Connection.CloseConnection(connection);
            return allStudent;
        }
    }
}
