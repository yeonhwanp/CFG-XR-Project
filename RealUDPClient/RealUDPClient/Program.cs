using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.IO;
using ProtoBuf;

namespace RealUDPClient
{
    /// <summary>
    /// This class contains the methods necessary to start the client.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {

            // Private constants 
            bool _done = false;
            const int listenPort = 12000;
            const int sendPort = 11000;

            // To listen for a response from the server
            UdpClient listener = new UdpClient(listenPort);

            while (!_done)
            {
                // Setting the networking stuff up here
                Socket thisSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                IPAddress target = IPAddress.Parse("127.0.0.1");
                IPEndPoint theEndPoint = new IPEndPoint(target, sendPort);

                // Serializing the data and sending it (atm just sends a test book obj)
                byte[] sendObject = Protobuf.SerializeData();
                thisSocket.SendTo(sendObject, theEndPoint);

                SendBack(_done, listener, thisSocket, theEndPoint);
            }
        }

        static void SendBack(bool _done, UdpClient listener, Socket sendSocket, IPEndPoint theEndPoint)
        {
            Console.WriteLine("Message sent to the broadcast address.");
            Console.WriteLine("Would you like to wait for a response?");
            string userResponse = Console.ReadLine();

            if (userResponse.ToLower() == "no")
                _done = true;
            else
            {
                byte[] sendMessage = Encoding.ASCII.GetBytes("SENDBACK");
                sendSocket.SendTo(sendMessage, theEndPoint);
                Console.WriteLine("Waiting...");

                byte[] receivedData = listener.Receive(ref theEndPoint);
                MemoryStream tempStream = new MemoryStream(receivedData);
                Book receivedBook = Serializer.Deserialize<Book>(tempStream);

                Console.WriteLine("Here is the data:");
                Console.WriteLine(receivedBook.ToString());
                Console.WriteLine(Encoding.ASCII.GetString(receivedData, 0, receivedData.Length));
                Console.ReadLine();
                



            }
        }
    }

    /// <summary>
    /// This class currently contains a test method for serializing data to send over.
    /// </summary>
    class Protobuf
    {
        public static byte[] SerializeData()
        {
            using (var testStream = new MemoryStream())
            {
                Book testBook = methods.GetData();
                Serializer.Serialize(testStream, testBook);
                byte[] returning = testStream.ToArray();
                return returning;
            }
        }
    }
}
