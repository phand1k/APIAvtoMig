using System.ComponentModel.DataAnnotations;

namespace APIAvtoMig.Task.Models
{
    public class Product
    {
        public Guid Id { get; set; }
        [MaxLength(255)]
        public string Name { get; set; }
        public string? Description { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
