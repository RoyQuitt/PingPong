using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using PingPong.Server.DTOs;

namespace PingPong.Server
{
    public class AsyncSocketServer
    {
        private int ServerPort { get; set; }
        public const int BUFFER_SIZE = 1024;
        public ManualResetEvent AllDone { get; set; }


        public AsyncSocketServer(int serverPort)
        {
            AllDone = new ManualResetEvent(false);
            ServerPort = serverPort;
        }

        private IPAddress GetLocalIPAddress()
        {
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in ipHostInfo.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip;
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        public void StartListening()
        {
            // Data buffer
            byte[] bytes = new byte[BUFFER_SIZE];

            IPAddress ipAddress = GetLocalIPAddress();
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, ServerPort);

            Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                Console.WriteLine(localEndPoint.ToString());
                listener.Bind(localEndPoint);
                listener.Listen(100);

                while(true)
                {
                    AllDone.Reset();

                    Console.WriteLine("Waiting for a connection...");
                    listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);

                    AllDone.WaitOne();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            Console.WriteLine($"{Environment.NewLine}Press ENTER to continue");
            Console.Read();
        }

        public void AcceptCallback(IAsyncResult asyncResult)
        {
            AllDone.Set();

            Socket listner = (Socket)asyncResult.AsyncState;
            Socket handler = listner.EndAccept(asyncResult);

            ClientData clientData = new ClientData();
            clientData.ClientSocket = handler;
            handler.BeginReceive(clientData.buffer, 0, ClientData.BufferSize, 0, new AsyncCallback(ReadCallback),
                                 clientData);
        }

        public void ReadCallback(IAsyncResult asyncResult)
        {
            string content = string.Empty;

            ClientData clientData = (ClientData)asyncResult.AsyncState;
            Socket handler = clientData.ClientSocket;

            int bytesRead = handler.EndReceive(asyncResult);

            if(bytesRead > 0)
            {
                clientData.StringBuilder.Append(Encoding.ASCII.GetString(clientData.buffer, 0, bytesRead));

                content = clientData.StringBuilder.ToString();
                if(content.IndexOf("<EOF>") > -1)
                {
                    Console.WriteLine($"Read {content.Length} bytes from socket{Environment.NewLine}" +
                        $"Data: {content}");

                    Send(handler, content);
                }
                else
                {
                    handler.BeginReceive(clientData.buffer, 0, ClientData.BufferSize, 0, new AsyncCallback(ReadCallback),
                                         clientData);
                }
            }
        }

        private void Send(Socket handler, string data)
        {
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            handler.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), handler);
        }

        private void SendCallback(IAsyncResult asyncResult)
        {
            try
            {
                Socket handler = (Socket)asyncResult.AsyncState;

                int bytesSent = handler.EndSend(asyncResult);
                Console.WriteLine($"Sent {bytesSent} bytes to client");

                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
