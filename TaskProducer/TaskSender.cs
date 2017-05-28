using RabbitMQ.Client;
using System;
using System.Text;

namespace TaskProducer
{
    public class TaskSender
    {
        public static void Main(string[] args)
        {
            string queueName = "taskQueue";

            Console.WriteLine("I'm a producer...");

            bool send = true;

            do
            {
                Console.WriteLine("Enter your message (no. of dots is the no. of sleep seconds):");
                string message = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(message))
                {
                    send = false;
                }
                else
                {
                    SendMessages(queueName, message);
                }

            } while (send);

            Console.WriteLine("Press [enter] to exit.");
            Console.ReadLine();
        }

        private static void SendMessages(string queueName, string message)
        {
            ConnectionFactory factory = new ConnectionFactory() { HostName = "localhost" };
            using (IConnection connection = factory.CreateConnection())
            using (IModel channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: queueName,
                                     durable: true, // queue won't be lost even if RabbitMQ restarts
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var body = Encoding.UTF8.GetBytes(message);

                // messages won't be lost - saves messages on disk/cache
                IBasicProperties properties = channel.CreateBasicProperties();
                properties.Persistent = true;

                channel.BasicPublish(exchange: "",
                                     routingKey: queueName,
                                     basicProperties: properties,
                                     body: body);

                Console.WriteLine("[x] Sent {0}", message);
            }
        }
    }
}
