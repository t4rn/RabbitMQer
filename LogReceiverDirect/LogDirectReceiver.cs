using DataModel;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace LogReceiverDirect
{
    public class LogDirectReceiver
    {
        public static void Main(string[] args)
        {
            string exchangeName = "logs_direct";

            Console.WriteLine("I'm a direct log receiver...");
            Console.WriteLine("Enter log types which you want to view seperated by space [info, warning, error]:");
            string input = Console.ReadLine();
            string[] logLevels = input.Split(' ');

            ConnectionFactory factory = new ConnectionFactory() { HostName = "localhost" };
            using (IConnection connection = factory.CreateConnection())
            {
                using (IModel channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare(exchange: exchangeName, type: "direct");

                    // non-durable, exclusive, autodelete queue with a generated name
                    string queueName = channel.QueueDeclare().QueueName;

                    foreach (string logLevel in logLevels)
                    {
                        channel.QueueBind(queue: queueName,
                                          exchange: exchangeName,
                                          routingKey: logLevel);
                    }

                    Console.WriteLine("[*] Waiting for logs.");

                    EventingBasicConsumer consumer = new EventingBasicConsumer(channel);
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
            byte[] body = e.Body;
            string message = Encoding.UTF8.GetString(body);
            Log log = Serializer.DeserializeFromJson<Log>(message);

            Console.WriteLine("[x] Received '{0}': '{1}'", log.Level, log.Message);
        }
    }
}
