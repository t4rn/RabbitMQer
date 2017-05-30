using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogBroadcaster
{
    public class LogSender
    {
        public static void Main(string[] args)
        {
            string exchangeName = "logs";

            Console.WriteLine("I'm a log broadcaster...");

            bool send = true;

            do
            {
                Console.WriteLine("Enter your log message. Press [enter] to exit:");
                string message = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(message))
                {
                    send = false;
                }
                else
                {
                    SendMessages(exchangeName, message);
                }

            } while (send);
        }

        private static void SendMessages(string exchangeName, string message)
        {
            ConnectionFactory factory = new ConnectionFactory() { HostName = "localhost" };
            using (IConnection connection = factory.CreateConnection())
            {
                using (IModel channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare(exchange: exchangeName, type: "fanout");

                    byte[] body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish(exchange: exchangeName,
                                         routingKey: "", // value ignored for fanout exchanges
                                         basicProperties: null,
                                         body: body);

                    Console.WriteLine("[x] Sent '{0}'", message);
                }
            }
        }
    }
}
