using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using Google.Protobuf;

// Notes: Abstract the protobuf method to <T>
// Notes: Two separate streams for UDP and TCP.
// Notes: Need to test receiving TCP when receiving UDP. Otherwise good (?)

namespace Servers
{
    /// <summary>
    /// Object to store stuff for TCPAsyncListener
    /// </summary>

    public class StartBoth
    {

        // Ports to be used
        private const int TCPInPort = 15000;
        private const int UDPStructureInPort = 8888;
        private const int UDPStructureOutPort = 9999;
        private const int UDPPositionInPort = 7777;
        private const int UDPPositionOutPort = 6666;
        private const int UDPMeshInPort = 1234;

        #region TCP stuff
        // TCP network stuff 
        //static IPAddress local = IPAddress.Parse("127.0.0.1");
        //static IPEndPoint TCPEndPoint = new IPEndPoint(local, TCPInPort);
        //static Socket TCPlistener = new Socket(local.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        #endregion

        // UDP network stuff
        public static UdpClient UDPStructureListener = new UdpClient(UDPStructureInPort);
        public static UdpClient UDPPositionListener = new UdpClient(UDPPositionInPort);
        public static UdpClient UDPMeshListener = new UdpClient(UDPMeshInPort);

        public static bool _TCPlistening = false;
        public static bool _Structurelistening = false;
        public static bool _Positionlistening = false;
        public static bool _MeshListening = false;


        public static void StartListening()
        {

            // Doing some stuff at start
            //TCPlistener.Bind(TCPEndPoint);
            Console.WriteLine("Waiting for connections...");

            while (true)
            {
                try
                {

                    #region TCP stuff
                    //// Don't know what this code is for but it was there before
                    //TCPlistener.Listen(100);

                    //// Only activate the listeners if they're not already launched. If they are, just skips right over.
                    //if (!_TCPlistening)
                    //{
                    //    _TCPlistening = true;
                    //    TCPlistener.BeginAccept(new AsyncCallback(TCPAsyncListener.AcceptCallBack), TCPlistener);
                    //}   
                    #endregion

                    // Start the individual servers if they're not listening on different threads

                    if (!_Structurelistening)
                    {
                        _Structurelistening = true;
                        UDPStructureListener.BeginReceive(new AsyncCallback(UDPAsyncListener<RobotStructure>.ReadCallBack), UDPStructureListener);
                    }

                    if (!_Positionlistening)
                    {
                        _Positionlistening = true;
                        UDPPositionListener.BeginReceive(new AsyncCallback(UDPAsyncListener<PositionList>.ReadCallBack), UDPPositionListener);
                    }

                    if (!_MeshListening)
                    {
                        _MeshListening = true;
                        UDPMeshListener.BeginReceive(new AsyncCallback(UDPAsyncListener<MeshList>.ReadCallBack), UDPMeshListener);
                    }
                }

                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }

        /// <summary>
        /// For a cleaner interface + verification that process was indeed finshed
        /// </summary>
        public static void ListeningMessage()
        {
            Console.WriteLine("---------------------");
            Console.WriteLine("Waiting for Connection...");
        }
    }

#if end

    /// <summary>
    /// Object for helping the TCPlistener.
    /// </summary>
    public class StateObject
    {
        public Socket workSocket = null;
        public const int Buffersize = 1024;
        public byte[] buffer = new byte[Buffersize];
        public StringBuilder sb = new StringBuilder();
    }

    /// <summary>
    /// Methods needed to receive/sendback data.
    /// Port: 15000
    /// </summary>
    public class TCPAsyncListener
    {
        /// <summary>
        /// Here we go after we get a signal.
        /// </summary>
        public static void AcceptCallBack(IAsyncResult ar)
        {

            Console.WriteLine("-----------------------");
            Console.WriteLine("TCP Connection made. Waiting for information...");

            // Socket that was plugged in
            Socket listener = (Socket)ar.AsyncState;
            // Completed the server thing -- now this socket contains all of the information
            Socket handler = listener.EndAccept(ar);

            // Create a stateobject to hold the socket
            StateObject state = new StateObject();
            state.workSocket = handler;
            
            // There's a buffer, buffersize... etc. Passing in state.
            handler.BeginReceive(state.buffer, 0, StateObject.Buffersize, 0, new AsyncCallback(ReadCallBack), state);
        }

        public static void ReadCallBack(IAsyncResult ar)
        {
            String content = String.Empty;

            // There's an asynchronous state object -- get the state object and handler object
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;

            // Endreceive stores number of bytes received.
            int bytesRead = handler.EndReceive(ar);

            // Append to the stringbuilder -> print out the result
            state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));
            content = state.sb.ToString();
            Console.WriteLine("\nThe client sent this: {0}\n", content);
            Send(handler, content);
        }

        public static void Send(Socket handler, String data)
        {
            byte[] byteData = Encoding.ASCII.GetBytes(data);
            handler.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallBack), handler);
        }

        private static void SendCallBack(IAsyncResult ar)
        {
            Socket handler = (Socket)ar.AsyncState;

            int bytesSent = handler.EndSend(ar);
            Console.WriteLine("Sent {0} bytes to client.", bytesSent);

            handler.Shutdown(SocketShutdown.Both);
            handler.Close();

            StartBoth._TCPlistening = false;
            StartBoth.ListeningMessage();
        }
    }

#endif 
    /// <summary>
    /// A class to handle all of the UDP connections.
    /// </summary>

    // But the question is... am i even using <T>? Am I doing this right?

    public class UDPAsyncListener<T> where T : IMessage<T>, new()
    {

        private static int listenPort;
        private static int replyPort;
        private static RobotStructure receivedStructure;
        private static PositionList receivedList;
        private static MeshList receivedMeshes;


        /// <summary>
        /// Goes here after receiving a signal
        /// </summary>
        public static void ReadCallBack(IAsyncResult res)
        {
            UdpClient client = (UdpClient)res.AsyncState;
            IPEndPoint RemoteIPEndPoint = new IPEndPoint(IPAddress.Any, listenPort);
            Console.WriteLine("----------------------");
            Console.WriteLine("UDP Connection made. Receiving data...");

            byte[] received = client.EndReceive(res, ref RemoteIPEndPoint);
            listenPort = ((IPEndPoint)client.Client.LocalEndPoint).Port;
            Console.WriteLine("Data received from: {0} at Port: {1}", RemoteIPEndPoint.Address.ToString(), listenPort.ToString());
            Console.WriteLine("Sending back changed position for debugging/testing purposes...");

            using (MemoryStream ms = new MemoryStream())
            {
                ms.Write(received, 0, received.Length);
                ms.Position = 0;

                if (listenPort == 8888)
                {
                    replyPort = 9999;
                    MessageParser<RobotStructure> parser = new MessageParser<RobotStructure>(() => new RobotStructure());
                    receivedStructure = parser.ParseFrom(ms);
                    receivedStructure.RootJointID = 1;
                    ms.Position = 0;
                    SendBackStructure(receivedStructure);
                }

                else if (listenPort == 7777)
                {
                    replyPort = 6666;
                    MessageParser<PositionList> parser = new MessageParser<PositionList>(() => new PositionList());
                    receivedList = parser.ParseFrom(ms);
                    receivedList.PList[0].Rotation = 30;
                    ms.Position = 0;
                    SendBackPositions(receivedList);
                }

                else if (listenPort == 1234)
                {
                    replyPort = 4321;
                    MessageParser<MeshList> parser = new MessageParser<MeshList>(() => new MeshList());
                    receivedMeshes = parser.ParseFrom(ms);
                    Console.WriteLine("RECEIVED MESHES... LENGTH = {0}", receivedMeshes.Meshes.Count);
                    StartBoth.ListeningMessage();
                    StartBoth._MeshListening = false;
                }

            }
        }

        public static void SendBackStructure(RobotStructure received)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                received.WriteTo(ms);
                Socket tempSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                IPEndPoint replyEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), replyPort);
                ms.Position = 0;
                tempSocket.SendTo(received.ToByteArray(), replyEndPoint);
                tempSocket.Close();
            }

            Console.WriteLine("Data sent!");
            StartBoth.ListeningMessage();
            StartBoth._Structurelistening = false;
        }

        public static void SendBackPositions(PositionList received)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                received.WriteTo(ms);
                Socket tempSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                IPEndPoint replyEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), replyPort);
                ms.Position = 0;
                tempSocket.SendTo(received.ToByteArray(), replyEndPoint);
                tempSocket.Close();
            }

            Console.WriteLine("Data sent!");
            StartBoth.ListeningMessage();
            StartBoth._Positionlistening = false;
        }
    }
}

