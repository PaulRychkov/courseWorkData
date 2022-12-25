namespace CourseWorkData.Models
{
    public class AppointmentTable
    {
        public int Id { get; set; }
        public int? Cabinet { get; set; }
        public int? District { get; set; }
        public DayOfWeek? Day { get; set; }
  
        public string AppStart { get; set; }
        
        public string AppEnd { get; set; }
        public string Lname { get; set; }
        public string Fname { get; set; }
        public string Pname { get; set; }
        public AppointmentTable(int Id, string Lname, string Fname, string Pname)
        {
            this.Id = Id;
            this.Lname = Lname;
            this.Fname = Fname;
            this.Pname = Pname;
        }
        public AppointmentTable(int Id, string Lname, string Fname, string Pname, int Cabinet, int District, DayOfWeek Day, string AppStart, string AppEnd)
        {
            this.Id = Id;
            this.Lname = Lname;
            this.Fname = Fname;
            this.Pname = Pname;
            this.Cabinet = Cabinet;
            this.District = District;
            this.Day = Day;
            this.AppStart = AppStart;
            this.AppEnd = AppEnd;
        }
    }
}
