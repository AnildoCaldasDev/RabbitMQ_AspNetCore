using RabbitMQ.Client;
using System;
using System.Text;

namespace RProducer
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "qWebAppRabbitMq",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                int counter = 0;
                while (counter < 1000)
                {
                    string message = $"{counter ++} - Enviado para consumidor";
                    var body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish(exchange: "", routingKey: "qWebAppRabbitMq", basicProperties: null, body: body);

                    Console.WriteLine("Sent {0}", message);
                    System.Threading.Thread.Sleep(500);
                }
            }

            Console.WriteLine(" Press Enter to Exit");
            Console.ReadLine();
        }
    }
}
