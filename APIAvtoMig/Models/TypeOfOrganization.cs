namespace APIAvtoMig.Models
{
    public class TypeOfOrganization
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Organization> Organizations { get; set; }
        public TypeOfOrganization()
        {
            Organizations = new List<Organization>();
        }
    }
}
