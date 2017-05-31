using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace LogReceiver
{
    public class LogReceiver
    {
        public static void Main(string[] args)
        {
            string exchangeName = "logs";

            Console.WriteLine("I'm a log receiver...");

            ConnectionFactory factory = new ConnectionFactory() { HostName = "localhost" };
            using (IConnection connection = factory.CreateConnection())
            {
                using (IModel channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare(exchange: exchangeName, type: "fanout");

                    // non-durable, exclusive, autodelete queue with a generated name
                    string queueName = channel.QueueDeclare().QueueName;

                    channel.QueueBind(queue: queueName,
                                      exchange: exchangeName,
                                      routingKey: "");

                    Console.WriteLine("[*] Waiting for logs.");

                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += Consumer_Received;

                    channel.BasicConsume(queue: queueName,
                                         noAck: true,
                                         consumer: consumer);

                    Console.WriteLine(" Press [enter] to exit.");
                    Console.ReadLine();
                }
            }
        }

        private static void Consumer_Received(object sender, BasicDeliverEventArgs e)
        {
            var body = e.Body;
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine("[x] Received: '{0}'", message);
        }
    }
}
