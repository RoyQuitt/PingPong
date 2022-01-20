using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;


namespace PingPong.Server.DTOs
{
    public class ClientData
    {
        public Socket ClientSocket = null;

        public const int BufferSize = 1024;

        public byte[] buffer = new byte[BufferSize];

        public StringBuilder StringBuilder = new StringBuilder();
    }
}
