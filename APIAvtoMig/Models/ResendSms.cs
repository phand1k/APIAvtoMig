using System.ComponentModel.DataAnnotations;

namespace APIAvtoMig.Models
{
    public class ResendSms
    {
        [Required(ErrorMessage = "Phone number is required!")]
        public string PhoneNumber { get; set; }
    }
}
