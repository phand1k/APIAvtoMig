using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;

public class PushNotificationService
{
    public async Task SendPushNotification(string deviceToken, string title, string body)
    {
        // Инициализация Firebase Admin SDK с использованием сервисного аккаунта
        FirebaseApp.Create(new AppOptions()
        {
            Credential = GoogleCredential.FromFile(@"C:\Users\Nursultan\source\repos\APIAvtoMig\APIAvtoMig\avtosat-383b5-firebase-adminsdk-77o9w-8750622c34.json")
        });

        // Создание сообщения для отправки
        var message = new Message()
        {
            Token = deviceToken, // Токен устройства получателя
            Notification = new Notification()
            {
                Title = title,
                Body = body
            }
        };

        // Отправка уведомления
        var response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
        Console.WriteLine("Successfully sent message: " + response);
    }
}
