using System.ComponentModel.DataAnnotations.Schema;

namespace APIAvtoMig.Models
{
    public class WashOrder
    {
        public int Id { get; set; }
        [ForeignKey("CarId")]
        public int? CarId { get; set; }
        public Car? Car { get; set; }
        [ForeignKey("ModelCarId")]
        public int? ModelCarId { get; set; }
        public ModelCar? ModelCar { get; set; }
        public string? CarNumber { get; set; }
        public bool? IsDeleted { get; set; } = false;
        public bool? IsOvered { get; set; } = false;
        [ForeignKey("OrganizationId")]
        public int? OrganizationId { get; set; }
        public Organization? Organization { get; set; }
        [ForeignKey("AspNetUserId")]
        public string? AspNetUserId { get; set; }
        public AspNetUser? AspNetUser { get; set; }
    }
}
