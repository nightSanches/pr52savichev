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
    public class WorkContext : Work
    {
        public WorkContext(int Id, int IdDiscipline, int IdType, DateTime Date, string Name, int Semester) :
            base(Id, IdDiscipline, IdType, Date, Name, Semester)
        { }
        public static List<WorkContext> AllWorks()
        {
            List<WorkContext> allWorks = new List<WorkContext>();
            MySqlConnection connection = Connection.OpenConnection();
            MySqlDataReader DBWork = Connection.Query("SELECT * FROM work ORDER BY Date", connection);
            while (DBWork.Read())
            {
                allWorks.Add(new WorkContext(
                    DBWork.GetInt32(0),
                    DBWork.GetInt32(1),
                    DBWork.GetInt32(2),
                    DBWork.GetDateTime(3),
                    DBWork.GetString(4),
                    DBWork.GetInt32(5)));
            }
            Connection.CloseConnection(connection);
            return allWorks;
        }
    }
}
