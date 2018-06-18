using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;


namespace RealUDPServer
{
    class Default
    {
        static void Main(string[] args)
        {

            UDPServer.StartListener();

        }
    }

    /// <summary>
    /// This class contains a method for starting up the UDP Server.
    /// </summary>
    public class UDPServer
    {

        private const int ListenPort = 11000;
        private const int ReplyPort = 12000;

        public static void StartListener()
        {

            bool done = false;

            UdpClient listener = new UdpClient(ListenPort);
            IPEndPoint other = new IPEndPoint(IPAddress.Any, ListenPort);

            try
            {
                while (!done)
                {
                    Console.WriteLine("Receiving data...");
                    byte[] initial = listener.Receive(ref other);

                    Console.WriteLine("Received data from {0}. \n {1} \n",
                                        other.ToString(),
                                        Encoding.ASCII.GetString(initial, 0, initial.Length));
                    Console.WriteLine("Waiting for sendback request...");

                    byte[] toSendBack = listener.Receive(ref other);
                    string theReply = Encoding.ASCII.GetString(toSendBack, 0, toSendBack.Length);

                    if (theReply.ToUpper() == "SENDBACK")
                    {
                        Console.WriteLine("Request for sendback receieved, sending back...");

                        SendBack(other.Address, initial, ReplyPort);
                    }

                    else
                    {
                        Console.WriteLine("Sendback request denied.");
                    }
                }

            }

            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            finally
            {
                listener.Close();
            }
        }

        public static void SendBack(IPAddress otherIP, byte[] data, int replyPort)
        {
            Socket thisSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint replyEndPoint = new IPEndPoint(otherIP, replyPort);

            Console.WriteLine("testing again {0}", data.Length);

            thisSocket.SendTo(data, replyEndPoint);
            Console.WriteLine("Data sent back.");
            Console.WriteLine();
        }

    }
}
