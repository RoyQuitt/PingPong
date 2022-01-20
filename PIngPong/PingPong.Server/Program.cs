using System;
using PingPong.Server;

namespace PingPong.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            var asyncSocketServer = new AsyncSocketServer(int.Parse(args[0]));
            asyncSocketServer.StartListening();
        }
    }
}
