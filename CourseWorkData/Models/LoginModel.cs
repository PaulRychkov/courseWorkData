using System.ComponentModel.DataAnnotations;

namespace CourseWorkData.Models
{
    public class LoginModel
    {
        [Key]
        [Required(ErrorMessage = "Не указан пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Не указана роль")]
        public Role URole { get; set; }
    }
    public enum Role
    {
        HeadPhysn,
        Doctor,
        Receptionist
    }
}
