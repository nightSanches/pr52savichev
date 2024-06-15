using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pr52savichev.Models
{
    public class Student
    {
        public int Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public int IdGroup { get; set; }
        public bool Expellend { get; set; }
        public DateTime DateExpelled { get; set; }
        public Student(int Id, string Firstname, string Lastname, int IdGroup, bool Expellend, DateTime DateExpelled)
        {
            this.Id = Id;
            this.Firstname = Firstname;
            this.Lastname = Lastname;
            this.IdGroup = IdGroup;
            this.Expellend = Expellend;
            this.DateExpelled = DateExpelled;
        }
    }
}
