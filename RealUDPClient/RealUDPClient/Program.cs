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
    class Program
    {
        static void Main(string[] args)
        {

            bool _done = false;
            const int listenPort = 12000;
            const int sendPort = 11000;

            UdpClient listener = new UdpClient(listenPort);

            while (!_done)
            {
                // Setting the sockets up
                Socket thisSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                IPAddress target = IPAddress.Parse("127.0.0.1");

                //byte[] Message = Encoding.ASCII.GetBytes("This is working!");

                byte[] sendObject = Protobuf.SerializeData();
                IPEndPoint theEndPoint = new IPEndPoint(target, sendPort);

                Console.WriteLine("testing 1 {0}", sendObject.Length);

                thisSocket.SendTo(sendObject, theEndPoint);

                Console.WriteLine("Message sent to the broadcast address.");
                Console.WriteLine("Would you like to wait for a response?");
                string userResponse = Console.ReadLine();

                if (userResponse.ToLower() == "no")
                    _done = true;
                else
                {

                    byte[] sendMessage = Encoding.ASCII.GetBytes("SENDBACK");
                    MemoryStream tempStream = new MemoryStream();

                    thisSocket.SendTo(sendMessage, theEndPoint);
                    Console.WriteLine("Waiting...");

                    byte[] receivedData = listener.Receive(ref theEndPoint);

                    tempStream.Position = 0;
                    tempStream.Write(receivedData, 0, receivedData.Length);

                    Console.WriteLine("Here is the data:");

                    Book receivedBook = Serializer.Deserialize<Book>(tempStream);

                    Console.WriteLine(receivedBook.ToString());
                    Console.WriteLine(Encoding.ASCII.GetString(receivedData, 0, receivedData.Length));
                    Console.ReadLine();
                }
            }
        }
    }

    class Protobuf
    {

        /// <summary>
        /// Test function: Returns a byte array of a serialized object.
        /// </summary>
        public static byte[] SerializeData()
        {
            // Creating necessary tools to serialize
            MemoryStream lStream = new MemoryStream();
            BinaryWriter lWriter = new BinaryWriter(lStream);

            // Test book object
            Book lBook = methods.GetData();

            // Serializing the object
            Serializer.Serialize(lStream, lBook);
            lWriter.Flush();
            lStream.Position = 0;

            byte[] returning = lStream.ToArray();

            lWriter.Close();
            lStream.Close();

            return returning;
        }
    }
}
