using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NetCoreApiRabbitMQ.Domain;
using RabbitMQ.Client;

namespace NetCoreApiRabbitMQ.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {

        ILogger<OrderController> _logger;

        public OrderController(ILogger<OrderController> logger)
        {
            this._logger = logger;
        }

        [HttpGet]
        public ActionResult GetTest()
        {
            return Accepted("testado com sucesso!");
        }


        [HttpPost]
        public ActionResult InsertOrder(Order order)
        {
            try
            {

                //int counter = 1;

                //while(counter < 5000)
                //{
                    //order.OrderGuid = Guid.NewGuid().ToString();
                    //order.Price = (order.Price * counter);

                    //Inserir na fila:
                    var factory = new ConnectionFactory() { HostName = "localhost" };
                    using (var connection = factory.CreateConnection())
                    using (var channel = connection.CreateModel())
                    {
                        channel.QueueDeclare(queue: "orderQueue",
                                             durable: false,
                                             exclusive: false,
                                             autoDelete: false,
                                             arguments: null);

                        string message = JsonSerializer.Serialize(order);
                        var body = Encoding.UTF8.GetBytes(message);

                        channel.BasicPublish(exchange: "",
                                             routingKey: "orderQueue",
                                             basicProperties: null,
                                             body: body);
                        //System.Threading.Thread.Sleep(500);

                    }
                  // counter = counter++;
                    // fim inserir
                //}


                return Accepted(order);
            }
            catch (Exception ex)
            {
                _logger.LogError("Erro ao tentar incluir novo produto.", ex);
                return new StatusCodeResult(500);
            }
        }



    }
}
