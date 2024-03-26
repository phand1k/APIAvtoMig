using System.ComponentModel.DataAnnotations.Schema;

namespace APIAvtoMig.Models
{
    public class WashOrder : Order
    {
        [ForeignKey("CarId")]
        public int? CarId { get; set; }
        public Car? Car { get; set; }
        [ForeignKey("ModelCarId")]
        public int? ModelCarId { get; set; }
        public ModelCar? ModelCar { get; set; }
        public string? CarNumber { get; set; }

    }
}
