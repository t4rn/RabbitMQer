using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;

namespace TaskConsumer
{
    public class TaskReceiver
    {
        private static int messagesReceived = 0;

        public static void Main(string[] args)
        {
            string queueName = "taskQueue";

            ConnectionFactory factory = new ConnectionFactory() { HostName = "localhost" };

            using (IConnection connection = factory.CreateConnection())
            {
                using (IModel channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: queueName,
                                         durable: true, // queue won't be lost even if RabbitMQ restarts
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);

                    // don't dispatch a new message to this consumer until it has processed and acknowledged the previous one. 
                    channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                    EventingBasicConsumer consumer = new EventingBasicConsumer(channel);
                    consumer.Received += Consumer_Received;

                    channel.BasicConsume(queue: queueName,
                                         noAck: false,// waits for acknowledgment that message is done processing
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


            int dots = message.Split('.').Length - 1;
            Thread.Sleep(dots * 2000);
            Console.WriteLine("Done -> total messages received = {0}", messagesReceived);

            EventingBasicConsumer consumer = (EventingBasicConsumer)sender;

            // send info, that task has been completed
            consumer.Model.BasicAck(deliveryTag: e.DeliveryTag, multiple: false);
        }
    }
}
