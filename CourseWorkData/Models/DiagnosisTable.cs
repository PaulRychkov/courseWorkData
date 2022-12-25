namespace CourseWorkData.Models
{
    public class DiagnosisTable
    {
        public string Dname { get; set; }
        public string ULname { get; set; }
        public string UFname { get; set; }
        public string UPname { get; set; }
        public string DLname { get; set; }
        public string DFname { get; set; }
        public string DPname { get; set; }
        public DateTime DateD { get; set; }

        public DiagnosisTable(string Dname, string ULname, string UFname, string UPname, string DLname, string DFname, string DPname, DateTime DateD)
        {
            this.Dname = Dname;
            this.ULname = ULname;
            this.UPname = UPname;
            this.DLname = DLname;
            this.DFname = DFname;
            this.DPname = DPname;
            this.UFname = UFname;
            this.DateD = DateD;

        }
    }
}
