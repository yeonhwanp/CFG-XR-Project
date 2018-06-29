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

            // Testing UDP: Working
            Book testObject = methods.GetData();
            Initializer.sendType("127.0.0.1", testObject);

            // Testing TCP: Working
            Initializer.sendType("127.0.0.1", sendMessage: "Testing TCP client");

            Console.Read();

        }
    }

    /// <summary>
    /// Class for the UDP Client
    /// </summary>
    class UDPClient
    {

        public static void UDPStart(String ipAddress, Object sendObject)
        {
            // Initializing necessary variables
            const int listenPort = 9999;
            const int sendPort = 8888;

            // Initializing Network variables
            UdpClient listener = new UdpClient(listenPort);
            IPAddress target = IPAddress.Parse(ipAddress);
            Socket thisSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint theEndPoint = new IPEndPoint(target, sendPort);

            // Send the serialized data
            byte[] sendBytes = Sender.SerializeData(sendObject);
            thisSocket.SendTo(sendBytes, theEndPoint);

            SendBack(listener, thisSocket, theEndPoint);

            thisSocket.Close();
            listener.Close();
        }

        /// <summary>
        /// Sends a sendback request and processes the information received (more like a debug but can be used to receive other messages)
        /// </summary>
        static void SendBack(UdpClient listener, Socket sendSocket, IPEndPoint theEndPoint)
        {
            Console.WriteLine("UDP Message sent to the broadcast address.");
            Console.WriteLine("Would you like to wait for a response?");
            string userResponse = Console.ReadLine();

            if (userResponse.ToLower() == "no")
            {
                byte[] sendMessage = Encoding.ASCII.GetBytes("NO");
                sendSocket.SendTo(sendMessage, theEndPoint);
                Console.WriteLine("Exiting...");
            }
            else
            {
                byte[] sendMessage = Encoding.ASCII.GetBytes("SENDBACK");
                sendSocket.SendTo(sendMessage, theEndPoint);
                Console.WriteLine("Waiting for a response...");

                byte[] receivedData = listener.Receive(ref theEndPoint);
                MemoryStream tempStream = new MemoryStream(receivedData);
                Generic receivedBook = Serializer.Deserialize<Generic>(tempStream);

                Console.WriteLine("Here is the data: \n");

                Console.WriteLine(receivedBook.ToString());
            }
        }
    }

    /// <summary>
    /// Class representing the TCP client
    /// </summary>
    class TCPClient
    {
        private const int port = 15000;
        
        public static void TCPStart(String ipAddress, String sendMessage)
        {
            // Initializing required variables
            string local = ipAddress;

            Console.WriteLine(sendMessage);
            byte[] sendBytes = Encoding.ASCII.GetBytes(sendMessage);
            byte[] data = new byte[1024];

            // Initializing network variables
            IPAddress target = IPAddress.Parse(ipAddress);
            IPEndPoint ipEndPoint = new IPEndPoint(target, port);
            Socket client = new Socket(target.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            // Connect to server, send information, receive information.
            try
            {
                client.Connect(ipEndPoint);
                Console.WriteLine("Sending TCP message...");
                int n = client.Send(sendBytes);
                Console.WriteLine("Message sent! Waiting for response...");
                int m = client.Receive(data);
                Console.WriteLine("Received response: '{0}'", Encoding.ASCII.GetString(data, 0, m));
                client.Shutdown(SocketShutdown.Both);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            client.Close();
        }
    }

    /// <summary>
    /// Starts the whole process.
    /// </summary>
    class Initializer
    {
        public static void sendType(String ipAddress, Object sendObject = null, String sendMessage = null)
        {
            // Initializing required network variables
            const int initialPort = 11000;
            Socket UDPConfirmSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPAddress target = IPAddress.Parse(ipAddress);
            IPEndPoint initialPoint = new IPEndPoint(target, initialPort);

            Console.WriteLine("------------------------------------");
            Console.Write("Please enter your preference: ");
            string userMessage = Console.ReadLine();
            byte[] initialMessage = Encoding.ASCII.GetBytes(userMessage);

            // Breaks off into the different clients
            if (userMessage.ToUpper() == "UDP")
            {
                UDPConfirmSocket.SendTo(initialMessage, initialPoint);
                UDPClient.UDPStart(ipAddress, sendObject);
            }
            else if (userMessage.ToUpper() == "TCP")
            {
                UDPConfirmSocket.SendTo(initialMessage, initialPoint);
                TCPClient.TCPStart(ipAddress, sendMessage);
            }
            else
            {
                Console.WriteLine("That is not a valid option.");
                sendType(ipAddress, sendObject, sendMessage);
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
                Generic testBook = methods.GetData();
                Serializer.Serialize(testStream, testBook);
                byte[] returning = testStream.ToArray();
                return returning;
            }
        }
    }
}
