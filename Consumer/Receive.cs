using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace Consumer
{
    public class Receive
    {
        private static int messagesReceived = 0;

        public static void Main(string[] args)
        {
            string queueName = "messagesQueue";

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

                    EventingBasicConsumer consumer = new EventingBasicConsumer(channel);
                    consumer.Received += Consumer_Received;

                    channel.BasicConsume(queue: queueName,
                                         noAck: true,
                                         consumer: consumer);

                    Console.WriteLine("Press [enter] to exit.");
                    Console.ReadLine();
                }
            }
        }

        private static void Consumer_Received(object sender, BasicDeliverEventArgs e)
        {
            var body = e.Body;
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine("[x] Received {0}", message);
            messagesReceived++;
            Console.WriteLine("Total messages received = {0}", messagesReceived);
        }
    }
}
