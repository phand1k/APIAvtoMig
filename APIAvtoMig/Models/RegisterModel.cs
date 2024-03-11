using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APIAvtoMig.Models
{
    public class RegisterModel
    {


        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }
        [Required(ErrorMessage = "Organization Number is required")]
        public string? OrganizationId { get; set; }
        [Required(ErrorMessage = "Phone number is required")]
        public string? PhoneNumber { get; set; }
    }
}
