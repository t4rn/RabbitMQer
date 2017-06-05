using DataModel;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace RPCServer
{
    public class RpcServer
    {
        public static void Main(string[] args)
        {
            string queueName = "rpc_queue";

            Console.WriteLine("I'm a RPC server, waiting for requests...");

            ConnectionFactory factory = new ConnectionFactory() { HostName = "localhost" };
            using (IConnection connection = factory.CreateConnection())
            {
                using (IModel channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: queueName, durable: false,
                        exclusive: false, autoDelete: false, arguments: null);

                    // don't dispatch a new message to this consumer until it has processed and acknowledged the previous one.
                    channel.BasicQos(0, 1, false);

                    EventingBasicConsumer consumer = new EventingBasicConsumer(channel);
                    channel.BasicConsume(queue: queueName,
                                         noAck: false,
                                         consumer: consumer);

                    consumer.Received += Consumer_Received;

                    Console.WriteLine(" Press [enter] to exit.");
                    Console.ReadLine();
                }
            }
        }

        private static void Consumer_Received(object sender, BasicDeliverEventArgs ea)
        {
            string input = null;
            string response = null;
            int n = 0;
            EventingBasicConsumer consumer = (EventingBasicConsumer)sender;

            byte[] body = ea.Body;
            IBasicProperties props = ea.BasicProperties;

            IBasicProperties replyProps = consumer.Model.CreateBasicProperties();
            replyProps.CorrelationId = props.CorrelationId;

            try
            {
                input = Encoding.UTF8.GetString(body);
                Console.WriteLine(" [.] Received '{0}' for calculation...", input);

                n = int.Parse(input);
                response = new Fibonacci().Calculate(n).ToString();
                Console.WriteLine(" [.] Fibonacci from '{0}' is '{1}'", input, response);
            }
            catch (Exception e)
            {
                Console.WriteLine(" [.] Exception: {0}", e.Message);
                response = "";
            }
            finally
            {
                byte[] responseBytes = Encoding.UTF8.GetBytes(response);

                consumer.Model.BasicPublish(exchange: "", routingKey: props.ReplyTo,
                    basicProperties: replyProps, body: responseBytes);

                // send info, that message from client was received, processed and can be removed from queue
                consumer.Model.BasicAck(deliveryTag: ea.DeliveryTag,
                    multiple: false);
            }

            Console.WriteLine("[x] Received '{0}' -> sent '{1}' to '{2}' with CorrelationId: '{3}'",
                input, response, props.ReplyTo, replyProps.CorrelationId);
        }
    }
}
