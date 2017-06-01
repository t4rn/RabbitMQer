using DataModel;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace LogReceiverTopic
{
    public class LogTopicReceiver
    {
        public static void Main(string[] args)
        {
            string exchangeName = "logs_topic";

            Console.WriteLine("I'm a topic log receiver...");
            Console.WriteLine("Enter routing key - list of words, delimited by dots.");
            Console.WriteLine(" * can substitute for exactly one word.\r\n # can substitute for zero or more words.");
            Console.WriteLine("Example: core.error | core.* | *.info");

            string routingKey = Console.ReadLine();

            ConnectionFactory factory = new ConnectionFactory() { HostName = "localhost" };
            using (IConnection connection = factory.CreateConnection())
            {
                using (IModel channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare(exchange: exchangeName, type: "topic");

                    // non-durable, exclusive, autodelete queue with a generated name
                    string queueName = channel.QueueDeclare().QueueName;

                    channel.QueueBind(queue: queueName,
                                      exchange: exchangeName,
                                      routingKey: routingKey);


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

            Console.WriteLine("[x] Received '{0}' from '{1}': '{2}'", log.Level, log.Source, log.Message);
        }
    }
}
