using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace APIAvtoMig.Models
{
    public class ModelCar
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        [ForeignKey("CarId")]
        public int? CarId { get; set; }
        [JsonIgnore]
        public Car? Car { get; set; }
        public bool? IsDeleted { get; set; } = false;
        public string? CarName => Car?.Name;
    }
}
