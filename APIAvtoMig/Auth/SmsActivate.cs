namespace APIAvtoMig.Auth
{
    public class SmsActivate
    {
        public int Id { get; set; }
        public string? PhoneNumber { get; set; }
        public bool? IsUsed { get; set; } = false;
        public DateTime? DateOfGetSMS { get; set; } = DateTime.Now;
        public DateTime? DateOfEndSMS { get; set; }
        public int? Code { get; set; }
    }
}
