using DataModel;
using RabbitMQ.Client;
using System;
using System.Text;
using static DataModel.Log;

namespace LogSenderTopic
{
    public class LogTopicSender
    {
        public static void Main(string[] args)
        {
            string exchangeName = "logs_topic";

            Console.WriteLine("I'm a topic log sender...");

            bool send = true;

            Console.WriteLine("Enter your source, log level and message. Available levels - {0}",
                string.Join(";", Enum.GetNames(typeof(LogLevel))));
            Console.WriteLine("Example: core info hello | front warning warn | crm error err.\r\nPress [enter] to exit:");
            do
            {
                Console.Write("Message> ");
                string input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input))
                {
                    send = false;
                }
                else
                {
                    Log log = PrepareLogFromInput(input);
                    SendMessages(exchangeName, log);
                }

            } while (send);
        }

        private static Log PrepareLogFromInput(string message)
        {
            Log log = new Log();

            string[] x = message?.Split(' ');
            log.Source = x[0];
            log.Level = x?.Length > 1 ? (LogLevel)Enum.Parse(typeof(LogLevel), x[1]) : LogLevel.info;
            log.Message = x?.Length > 2 ? x[2] : "Empty message";

            return log;
        }

        private static void SendMessages(string exchangeName, Log log)
        {
            string routingKey = $"{log.Source}.{log.Level.ToString()}";

            ConnectionFactory factory = new ConnectionFactory() { HostName = "localhost" };
            using (IConnection connection = factory.CreateConnection())
            {
                using (IModel channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare(exchange: exchangeName, type: "topic");

                    string logSerialized = Serializer.SerializeToJson(log);

                    byte[] body = Encoding.UTF8.GetBytes(logSerialized);

                    channel.BasicPublish(exchange: exchangeName,
                                         routingKey: routingKey,
                                         basicProperties: null,
                                         body: body);

                    Console.WriteLine("[x] Sent '{0}': '{1}'", routingKey, log.Message);
                }
            }
        }
    }
}
