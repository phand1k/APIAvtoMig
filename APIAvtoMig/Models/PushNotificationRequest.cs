namespace APIAvtoMig.Models
{
    public class PushNotificationRequest
    {
        public string DeviceToken { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
    }
}
