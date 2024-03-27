using System.ComponentModel.DataAnnotations.Schema;

namespace APIAvtoMig.Models
{
    public class Service
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public bool? IsDeleted { get; set; } = false;
        [ForeignKey("AspNetUserId")]
        public string? AspNetUserId { get; set; }
        public AspNetUser? AspNetUser { get; set; }
        [ForeignKey("OrganizationId")]
        public int? OrganizationId { get; set; }
        public Organization? Organization { get; set; }
        public DateTime? DateOfCreatedService { get; set; } = DateTime.Now;

        public int? Price { get; set; } = 0;
    }
}
