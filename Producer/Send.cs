using RabbitMQ.Client;
using System;
using System.Text;

namespace CoreProducer
{
    public class Send
    {
        public static void Main(string[] args)
        {
            string queueName = "messagesQueue";

            Console.WriteLine("I'm a producer...");
            Console.WriteLine("How many messages to send?");

            bool send = true;
            int count = Convert.ToInt32(Console.ReadLine());
            do
            {
                SendMessages(queueName, count);

                Console.WriteLine("Messages send - how many more messages do you want to send?\r\nTo exit press any nonnumeric key...");

                send = int.TryParse(Console.ReadLine(), out count);

            } while (send);
        }

        /// <summary>
        /// Sends the given amount of messages to the given queue
        /// </summary>
        private static void SendMessages(string queueName, int amount)
        {
            ConnectionFactory factory = new ConnectionFactory() { HostName = "localhost" };

            using (IConnection connection = factory.CreateConnection())
            {
                using (IModel channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: queueName,
                                         durable: false,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);

                    for (int i = 1; i <= amount; i++)
                    {
                        string message = $"Message no. '{i}' - sending date = '{DateTime.Now}'";

                        var body = Encoding.UTF8.GetBytes(message);

                        channel.BasicPublish(exchange: "",
                                             routingKey: queueName,
                                             basicProperties: null,
                                             body: body);

                        Console.WriteLine("Sent {0}", message);
                    }
                }
            }
        }
    }
}
