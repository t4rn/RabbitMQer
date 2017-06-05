using System;

namespace RPCClient
{
    public class Start
    {
        public static void Main()
        {
            string exchangeName = "rpc_queue";

            Console.WriteLine("I'm a RPC client, enter positive integer for Fibonacci:");

            bool send = true;
            do
            {
                Console.Write("Number> ");
                string input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input))
                {
                    send = false;
                }
                else
                {
                    Send(exchangeName, input);
                }

            } while (send);

            Console.Read();
        }

        private static void Send(string queueName, string input)
        {
            RpcClient rpcClient = new RpcClient(queueName);
            rpcClient.Call(input);
        }
    }
}
