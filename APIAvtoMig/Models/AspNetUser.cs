using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace APIAvtoMig.Models
{
    public class AspNetUser : IdentityUser
    {
        [StringLength(40, ErrorMessage = "Имя не может содержать больше 40 символов и меньше 2-х символов", MinimumLength = 2)]
        public string? FirstName { get; set; }
        [StringLength(40, ErrorMessage = "Фамилия не может содержать больше 40 символов и меньше 2-х символов", MinimumLength = 2)]
        public string? LastName { get; set; }
        public DateTime DateOfCreated { get; set; } = DateTime.Now;
        [ForeignKey("OrganizationId")]
        public int? OrganizationId { get; set; }
        [JsonIgnore]
        public Organization? Organization { get; set; }
        public ICollection<WashOrder>? WashOrders { get; set; }
        public AspNetUser()
        {
            WashOrders = new List<WashOrder>();
        }
    }
}
