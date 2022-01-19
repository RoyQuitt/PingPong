using System;

namespace PIngPong.Client
{
    partial class Program
    {

        static void Main(string[] args)
        {
            var client = new SocketClient();
            client.ExecuteClient();
        }
    }
}
