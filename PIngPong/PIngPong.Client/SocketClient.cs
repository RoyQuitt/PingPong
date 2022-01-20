using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using PingPong.Utils;

namespace PIngPong.Client
{
    partial class Program
    {
        public class SocketClient
        {
            public int Port { get; set; }
            private IInput _reader;

            public SocketClient(int port, IInput reader)
            {
                Port = port;
                _reader = reader;
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

            private string GetStringToSend()
            {
                return _reader.Read();
            }

            public void ExecuteClient()
            {
                try
                {
                    IPAddress ipAddr = GetLocalIPAddress();
                    IPEndPoint localEndPoint = new IPEndPoint(ipAddr, Port);

                    Socket sender = new Socket(ipAddr.AddressFamily,
                               SocketType.Stream, ProtocolType.Tcp);

                    try
                    {

                        sender.Connect(localEndPoint);

                        Console.WriteLine("Socket connected to -> {0} ",
                                      sender.RemoteEndPoint.ToString());

                        byte[] messageSent = Encoding.ASCII.GetBytes("Test Client<EOF>");
                        int byteSent = sender.Send(messageSent);

                        byte[] messageReceived = new byte[1024];

                        int byteRecv = sender.Receive(messageReceived);
                        Console.WriteLine("Message from Server -> {0}",
                              Encoding.ASCII.GetString(messageReceived,
                                                         0, byteRecv));

                        sender.Shutdown(SocketShutdown.Both);
                        sender.Close();
                    }

                    catch (ArgumentNullException ane)
                    {

                        Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
                    }

                    catch (SocketException se)
                    {

                        Console.WriteLine("SocketException : {0}", se.ToString());
                    }

                    catch (Exception e)
                    {
                        Console.WriteLine("Unexpected exception : {0}", e.ToString());
                    }
                }

                catch (Exception e)
                {

                    Console.WriteLine(e.ToString());
                }

            }
        }
    }
}
