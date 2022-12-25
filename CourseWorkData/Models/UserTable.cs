using System.Xml.Linq;

namespace CourseWorkData.Models
{
    public class UserTable
    {
        public string Dname { get; set; }
        public string Lname { get; set; }
        public string Fname { get; set; }

        public string Pname { get; set; }
        public string Address { get; set; }
        public DateTime DateD { get; set; }

        public int Did { get; set; }

        public UserTable(string Address, string Dname, string Lname, string Fname, string Pname,  DateTime DateD, int did)
        {
            this.Address = Address;
            this.Lname = Lname;
            this.Pname = Pname;
            this.Fname = Fname;
            this.DateD = DateD;
            this.Dname = Dname;
            this.Did = did;
        }
    }
}
