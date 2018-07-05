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

public class ClientUDP
{
    // ATM just sends data over UDP (but not working because protobuf not workng)
    public static PositionList UDPSend(string ipAddress, PositionList sendObject)
    {
        const int listenPort = 9999;
        const int sendPort = 8888;

        Debug.Log("Doing...");
        UdpClient listener = new UdpClient(listenPort);
        IPAddress target = IPAddress.Parse(ipAddress);
        Socket thisSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        IPEndPoint theEndPoint = new IPEndPoint(target, sendPort);

        // Sending Data
        Debug.Log("Sending data...");
        using (MemoryStream ms = new MemoryStream())
        {
            sendObject.WriteTo(ms);
            thisSocket.SendTo(ms.ToArray(), theEndPoint);
        }
        Debug.Log("Data sent!");

        // Receiving new Data then returning it.
        byte[] receivedBytes = listener.Receive(ref theEndPoint);

        using (MemoryStream tempStream = new MemoryStream(receivedBytes))
        {
            // Writing the data received to the stream
            tempStream.Write(receivedBytes, 0, receivedBytes.Length);
            tempStream.Position = 0;

            // Deserializing
            PositionList receivedList = PositionList.Parser.ParseFrom(tempStream);

            // Closing and returning
            thisSocket.Close();
            listener.Close();
            return receivedList;
        }
    }
}

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