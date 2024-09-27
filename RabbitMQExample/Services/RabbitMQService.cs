using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQExample.Models;
using System.Data.Common;
using System.Text;
using System.Text.Json;

namespace RabbitMQExample.Services
{
    public class RabbitMQService
    {
        private readonly ConnectionFactory _factory;
        private readonly IConnection _connection;
        public RabbitMQService()
        {
            _factory = new ConnectionFactory { HostName = "localhost", UserName = "rabbitmq", Password = "rabbitmq" };
            _connection = _factory.CreateConnection();
        }
        public void Publish(Notification notification)
        {
             using var channel = _connection.CreateModel();
            channel.QueueDeclare(
                queue: "notifications",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            var json = JsonSerializer.Serialize(notification);
            var body = Encoding.UTF8.GetBytes(json);

            channel.BasicPublish(exchange: "", routingKey: "notifications", body: body);
            Console.WriteLine("Message Published");
        }

        public async Task ConsumerRabbitMQAsync()
        {
            try
            {

                using var channel = _connection.CreateModel();

                channel.QueueDeclare(queue: "notifications", durable: false, exclusive: false, autoDelete: false, arguments: null);


                var consumer = new EventingBasicConsumer(channel);

                consumer.Received += async (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var jsonNotification = Encoding.UTF8.GetString(body);
                    var notification = JsonSerializer.Deserialize<Notification>(jsonNotification);
                    if (notification != null)
                    {
                        Console.WriteLine(notification);

                    }
                };

                channel.BasicConsume(queue: "notifications", autoAck: true, consumer: consumer);

                await Task.Delay(1000);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }


        }

        //public void Consumer()
        //{
        //    var factory = new ConnectionFactory { HostName = "localhost", UserName = "rabbitmq", Password = "rabbitmq" };
        //    var connection = factory.CreateConnection();
        //    var channel = connection.CreateModel();

        //    channel.QueueDeclare(
        //        queue: "notifications",
        //        durable: false,
        //        exclusive: false,
        //        autoDelete: false,
        //        arguments: null
        //    );

        //    var consumer = new EventingBasicConsumer(channel);

        //    consumer.Received += (model, ea) =>
        //    {
        //        var body = ea.Body.ToArray();
        //        var message = Encoding.UTF8.GetString(body);
        //        Console.WriteLine($"Received: {message}");

        //        // Eğer autoAck=false ise, mesajı elle onaylayabilirsiniz.
        //        channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
        //    };

        //    // Mesajları tüketmeye başla, autoAck=false yaparak manuel onaylama aktif ettik
        //    channel.BasicConsume(queue: "notifications", autoAck: false, consumer: consumer);

        //    // Sürekli bir dinleme işlemi başlatmak için Task kullanın
        //    Task.Run(() => {
        //        Console.WriteLine("Listening for messages. Press [enter] to exit.");
        //        Console.ReadLine();  // Program çalışırken burada bekleyecek
        //    }).Wait();
        //}
    }
}
