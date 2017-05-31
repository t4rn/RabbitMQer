using DataModel;
using RabbitMQ.Client;
using System;
using System.Text;
using static DataModel.Log;

namespace LogSenderDirect
{
    public class LogDirectSender
    {
        public static void Main(string[] args)
        {
            string exchangeName = "logs_direct";

            Console.WriteLine("I'm a direct log sender...");

            bool send = true;

            Console.WriteLine("Enter your log level and message. Available levels - {0}",
                string.Join(";",Enum.GetNames(typeof(LogLevel))));
            Console.WriteLine("Example: info hello | warning warn | error err.\r\nPress [enter] to exit:");
            do
            {
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
            log.Level = x?.Length > 0 ? (LogLevel)Enum.Parse(typeof(LogLevel), x[0]) : LogLevel.info;
            log.Message = x?.Length > 1 ? x[1] : "Empty message";

            return log;
        }

        private static void SendMessages(string exchangeName, Log log)
        {
            string logLevel = log.Level.ToString();

            ConnectionFactory factory = new ConnectionFactory() { HostName = "localhost" };
            using (IConnection connection = factory.CreateConnection())
            {
                using (IModel channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare(exchange: exchangeName, type: "direct");

                    string logSerialized = Serializer.SerializeToJson(log);

                    byte[] body = Encoding.UTF8.GetBytes(logSerialized);

                    channel.BasicPublish(exchange: exchangeName,
                                         routingKey: logLevel,
                                         basicProperties: null,
                                         body: body);

                    Console.WriteLine("[x] Sent '{0}': '{1}'", logLevel, log.Message);
                }
            }
        }
    }
}
