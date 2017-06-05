using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace RPCClient
{
    public class RpcClient
    {
        private IConnection connection;
        private IModel channel;
        private string replyQueueName;
        private EventingBasicConsumer consumer;
        private readonly string queueName;

        public RpcClient(string queueName)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            // random generated queue name
            replyQueueName = channel.QueueDeclare().QueueName;
            consumer = new EventingBasicConsumer(channel);
            this.queueName = queueName;

            channel.BasicConsume(queue: replyQueueName,
                                 noAck: true,
                                 consumer: consumer);
        }

        public void Call(string input)
        {
            Console.WriteLine(" [x] Requesting Fibonacci from '{0}'", input);

            string corrId = Guid.NewGuid().ToString();
            IBasicProperties props = channel.CreateBasicProperties();
            props.ReplyTo = replyQueueName;
            props.CorrelationId = corrId;

            byte[] messageBytes = Encoding.UTF8.GetBytes(input);

            channel.BasicPublish(exchange: "",
                                 routingKey: queueName,
                                 basicProperties: props,
                                 body: messageBytes);

            consumer.Received += (sender, e) => Consumer_Received(sender, e, input, corrId);
        }

        private void Consumer_Received(object sender, BasicDeliverEventArgs ea, string input, string corrId)
        {
            // unique queue for client
            if (ea.BasicProperties.CorrelationId == corrId)
            {
                string response = Encoding.UTF8.GetString(ea.Body);
                Console.WriteLine("\r\n [.] Requested Fib for '{0}' -> result '{1}'",
                    input, response);
                Close();
            }
        }

        private void Close()
        {
            connection.Close();
        }
    }
}
