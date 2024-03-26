using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APIAvtoMig.Models
{
    public class Order
    {

        public int Id { get; set; }
        public bool? IsDeleted { get; set; } = false;
        public DateTime? DateOfCreated { get; set; } = DateTime.Now;
        [ForeignKey("OrganizationId")]
        public int? OrganizationId { get; set; }
        public Organization? Organization { get; set; }
        [ForeignKey("AspNetUserId")]
        public string? AspNetUserId { get; set; }
        public AspNetUser? AspNetUser { get; set; }

        public bool? IsOvered { get; set; } = false;
    }
}
