using System.Text.Json.Serialization;

namespace APIAvtoMig.Models
{
    public class Car
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        [JsonIgnore]
        public ICollection<ModelCar>? ModelCars { get; set; }
        public bool? IsDeleted { get; set; } = false;
        public Car()
        {
            ModelCars = new List<ModelCar>();
        }
    }
}
