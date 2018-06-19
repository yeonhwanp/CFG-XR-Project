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

            Initializer.sendType();

        }
    }

    class UDPClient
    {

        public static void UDPStart()
        {
            bool _done = false;
            const int listenPort = 9999;
            const int sendPort = 8888;

            UdpClient listener = new UdpClient(listenPort);
            IPAddress target = IPAddress.Parse("127.0.0.1");

            while (!_done)
            {
                // Setting the networking stuff up here
                Socket thisSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                IPEndPoint theEndPoint = new IPEndPoint(target, sendPort);

                // Serializing the data and sending it (atm just sends a test book obj)
                // Would we want to read a file then send some classes over or how do we want to do this? 
                Book toSend = methods.GetData();
                byte[] sendObject = Sender.SerializeData(toSend);
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

    class TCPClient
    {
        private const int port = 15000;
        
        public static void TCPStart()
        {
            string local = "127.0.0.1";
            string sendText = "TCPClient works!";
            byte[] sendBytes = Encoding.ASCII.GetBytes("TCPClient works!");
            byte[] data = new byte[1024];

            IPAddress target = IPAddress.Parse("127.0.0.1");
            IPEndPoint ipEndPoint = new IPEndPoint(target, port);

            Socket client = new Socket(target.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                client.Connect(ipEndPoint);
                int n = client.Send(sendBytes);
                int m = client.Receive(data);
                Console.WriteLine("Received: {0}", Encoding.ASCII.GetString(data, 0, data.Length));
                client.Shutdown(SocketShutdown.Both);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            client.Close();
            Console.Read();
        }
    }

    /// <summary>
    /// Starts the whole process.
    /// </summary>
    class Initializer
    {
        public static void sendType()
        {
            const int initialPort = 11000;
            Socket UDPConfirmSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPAddress target = IPAddress.Parse("127.0.0.1");
            IPEndPoint initialPoint = new IPEndPoint(target, initialPort);

            Console.WriteLine("Please enter your preference: TCP/UDP");
            string userMessage = Console.ReadLine();
            byte[] initialMessage = Encoding.ASCII.GetBytes(userMessage);

            if (userMessage.ToUpper() == "UDP")
            {
                UDPConfirmSocket.SendTo(initialMessage, initialPoint);
                UDPClient.UDPStart();
            }
            else if (userMessage.ToUpper() == "TCP")
            {
                UDPConfirmSocket.SendTo(initialMessage, initialPoint);
                TCPClient.TCPStart();
            }
            else
            {
                Console.WriteLine("That is not a valid option.");
                sendType();
            }
        }


        
    }

    /// <summary>
    /// This class currently contains a test method for serializing data to send over.
    /// </summary>
    class Sender
    {
        /// <summary>
        /// Actual method returns a byte array of the serialized object passed in.
        /// </summary>
        public static byte[] SerializeData<T>(T arg)
        {
            try
            {
                using (var testStream = new MemoryStream())
                {
                    Serializer.Serialize(testStream, arg);
                    byte[] returning = testStream.ToArray();
                    return returning;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                byte[] sendError = Encoding.ASCII.GetBytes(e.ToString());
                return sendError;
            }
        }

        /// <summary>
        /// The method that tests serialization with a book object.
        /// </summary>
        public static byte[] testSerialization()
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
