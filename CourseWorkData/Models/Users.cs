using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CourseWorkData.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Не указана фамилия")]
        [StringLength(30, MinimumLength = 2, ErrorMessage = "Длина фамилии должна быть от 2 до 30 символов")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Не указано имя")]
        [StringLength(30, MinimumLength = 2, ErrorMessage = "Длина имени должна быть от 2 до 30 символов")]
        public string FirstName { get; set; }
        
        [StringLength(30, MinimumLength = 2, ErrorMessage = "Длина отчества должна быть от 2 до 30 символов")] 
        public string? PatrName { get; set; }

        [Required(ErrorMessage = "Не указан адрес")]
        [StringLength(100, MinimumLength = 10, ErrorMessage = "Длина адреса должна быть от 10 до 100 символов")]
        public string Adress { get; set; }

        public List<Diagnosis>? Diagnosis { get; set; }

    }
    public class Disease
    {
        [Key]
        public int Id { get; set; }
       [Required(ErrorMessage = "Не указано название болезни")]
       [StringLength(100, MinimumLength = 2, ErrorMessage = "Длина болезни должна быть от 2 до 100 символов")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Не указан список симптомов")] 
        public List<Symptom> Symptom { get; set; }

        [Required(ErrorMessage = "Не указан список лекарств")] 
        public List<Medicine> Medicine { get; set; }

        public List<Diagnosis>? Diagnosis { get; set; }
    }

    public class Symptom
    {
        public int Id { get; set; }


        [Required(ErrorMessage = "Не указан симптом")]
        [StringLength(500, MinimumLength = 2, ErrorMessage = "Длина симптома должна быть от 2 до 500 символов")]
        public string? Name { get; set; }

        public int? DiseaseId { get; set; }

        public  Disease? Disease { get; set; }
    }
    public class Medicine
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Не указано лекарство")]
        [StringLength(500, MinimumLength = 2, ErrorMessage = "Длина лекарства должна быть от 2 до 500 символов")]
        public string? Name { get; set; }
        public int? DiseaseId { get; set; }
        public Disease? Disease { get; set; }
    }
    public class Doctor
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Не указана фамилия")]
        [StringLength(30, MinimumLength = 2, ErrorMessage = "Длина фамилии должна быть от 2 до 30 символов")]
        public string LastName { get; set; }
       
        [Required(ErrorMessage = "Не указано имя")]
        [StringLength(30, MinimumLength = 2, ErrorMessage = "Длина имени должна быть от 2 до 30 символов")]
        public string FirstName { get; set; }
        public string? PatrName { get; set; }
        public List<Diagnosis>? Diagnosis { get; set; }
        public List<Appointment>? Appointment { get; set; }
    }

    public class Appointment
    {
        public int Id { get; set; }
        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; }

        [Required(ErrorMessage = "Не указан участок")]
        [Range(1, 99, ErrorMessage = "Недопустимый участок")]   
        public int District { get; set; }

        
        [Required(ErrorMessage = "Не указан кабинет")] 
        [Range(101, 499, ErrorMessage = "Недопустимый кабинет")]
        public int Сabinet { get; set; }

        [Required(ErrorMessage = "Не указан день недели")] 
        public DayOfWeek Day { get; set; }

        [Required(ErrorMessage = "Не указано начало приема")] 
        public DateTime AppStart { get; set; }

        [Required(ErrorMessage = "Не указано окончание")]
        public DateTime AppEnd { get; set; }
    }
    public enum DayOfWeek
    {
        Friday,
        Monday,
        Saturday,
        Sunday,
        Thursday,
        Tuesday,
        Wednesday,
    }

    public class Diagnosis
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; }
        public int DiseaseId { get; set; }
        public Disease Disease { get; set; }

        [Required(ErrorMessage = "Не указана дата")]
        public DateTime Date { get; set; }
    }

 
    
}
