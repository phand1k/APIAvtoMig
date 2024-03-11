using System.ComponentModel.DataAnnotations.Schema;

namespace APIAvtoMig.Models
{
    public class Subscription
    {
        public int Id { get; set; }
        [ForeignKey("OrganizationId")]
        public int? OrganizationId { get; set; }
        public DateTime? DateOfPayment { get; set; } = DateTime.Now;
        public DateTime? DateOfStartSubscription { get; set; } = DateTime.Now;
        public DateTime? DateOfEndSubscription { get; set; }
    }
}
