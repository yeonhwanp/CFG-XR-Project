using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Linq;
using System.Text;
using System;
using Google.Protobuf;

public class ClientUDP<T> where T: IMessage<T>, new()
{
    public static T UDPSend(int sendPort, T sendObject)
    {
        int listenPort = 0;
        T received;

        IPAddress target = IPAddress.Parse("127.0.0.1");
        IPEndPoint theEndPoint = new IPEndPoint(target, sendPort);
        Socket thisSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        if (sendPort == 8888)
        {
            listenPort = 9999;
        }
        else if (sendPort == 7777)
        {
            listenPort = 6666;
        }

        UdpClient listener = new UdpClient(listenPort);

        Debug.Log("Sending data...");
        using (MemoryStream ms = new MemoryStream())
        {
            sendObject.WriteTo(ms);
            thisSocket.SendTo(ms.ToArray(), theEndPoint);
        }
        Debug.Log("Data sent!");
        byte[] receivedBytes = listener.Receive(ref theEndPoint);

        // Writing received data
        using (MemoryStream tempStream = new MemoryStream(receivedBytes))
        {
            // Writing the data received to the stream
            tempStream.Write(receivedBytes, 0, receivedBytes.Length);
            tempStream.Position = 0;

            // Deserializing
            MessageParser<T> parser = new MessageParser<T>(() => new T());
            received = parser.ParseFrom(tempStream);

            // Closing and returning
            thisSocket.Close();
            listener.Close();
            return received;
        }
    }
}

#if end
public class ClientTCP
{
    private const int port = 15000;

    public static void TCPSend(string ipAddress, string sendMessage)
    {
        string local = ipAddress;
        byte[] sendBytes = Encoding.ASCII.GetBytes(sendMessage);
        byte[] data = new byte[1024];

        IPAddress target = IPAddress.Parse(ipAddress);
        IPEndPoint ipEndPoint = new IPEndPoint(target, port);
        Socket client = new Socket(target.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

        try
        {
            client.Connect(ipEndPoint);
            Debug.Log("Sending TCP message...");
            int n = client.Send(sendBytes);
            int m = client.Receive(data);
            Debug.Log("Received response: " + Encoding.ASCII.GetString(data, 0, m));
            client.Shutdown(SocketShutdown.Both);
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }

        client.Close();
    }
}
#endif