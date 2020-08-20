using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RConsumerOrderWorker.Domain;
using System;
using System.Text;
using System.Text.Json;

namespace RConsumerOrderWorker
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "orderQueue",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    try
                    {
                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);
                        Order order = JsonSerializer.Deserialize<Order>(message);

                        if(order.OrderGuid == "9d870b4a-9b45-42eb-939f-873504b4668d" || order.OrderGuid == "e1d33f87-bda8-4768-88e4-e4ad2eeeacfa")
                        {
                            throw new ArgumentException(order.OrderGuid);
                        }

                        Console.WriteLine($"Order: {order.OrderGuid} | {order.OrderNumber} | {order.ItemName} | {order.Price:N2} ");

                        channel.BasicAck(ea.DeliveryTag, false);

                    }
                    catch (Exception)
                    {
                        //logger
                        channel.BasicNack(ea.DeliveryTag, false, true);
                    }

                };
                channel.BasicConsume(queue: "orderQueue",
                                     autoAck: false,
                                     consumer: consumer);

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }
    }
}
