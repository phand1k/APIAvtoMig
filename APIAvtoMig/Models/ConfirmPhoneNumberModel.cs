using System.ComponentModel.DataAnnotations;

namespace APIAvtoMig.Models
{
    public class ConfirmPhoneNumberModel
    {
        [Required(ErrorMessage = "Phone number is required")]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Code is required")]
        public int? Code { get; set; }
    }
}
