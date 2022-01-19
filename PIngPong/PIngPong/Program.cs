using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace PIngPong
{

    class Program
    {
        static void Main(string[] args)
        {
            var server = new SocketServer(int.Parse(args[0]));
            server.ExecuteServer();
        }
    }
}
