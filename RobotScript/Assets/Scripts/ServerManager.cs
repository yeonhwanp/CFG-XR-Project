using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Linq;
using System.Text;
using System;
using ProtoBuf;

/// <summary>
/// I dont think we can send/receive multiple at a time... Will have to see about that. Worst case scenario, implement Async.
/// </summary>

public class ClientUDP<T>
{
    // ATM just sends data over UDP (but not working because protobuf not workng)
    public static StorageProto<T> UDPSend(string ipAddress, StorageProto<T> sendObject)
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
        byte[] sendBytes = Sender.SerializeData(sendObject);
        thisSocket.SendTo(sendBytes, theEndPoint);
        Debug.Log("Data sent!");

        // Receiving new Data then returning it.
        byte[] receivedBytes = listener.Receive(ref theEndPoint);
        using (MemoryStream tempStream = new MemoryStream(receivedBytes))
        {
            StorageProto<T> receivedList = Serializer.Deserialize<StorageProto<T>>(tempStream);
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

/// <summary>
/// Serializes data
/// </summary>
class Sender
{
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
            Debug.Log(e.ToString());
            byte[] sendError = Encoding.ASCII.GetBytes(e.ToString());
            return sendError;
        }
    }
}

