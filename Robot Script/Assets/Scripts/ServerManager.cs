using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Linq;
using System.Text;
using System;

public class ServerManager {

    public PositionList sendList;

}

public class ClientUDP
{
    // ATM just sends data over UDP (but not working because protobuf not workng)
    public static void UDPSend(string ipAddress, System.Object sendObject)
    {
        const int listenPort = 9999;
        const int sendPort = 8888;

        UdpClient listener = new UdpClient(listenPort);
        IPAddress target = IPAddress.Parse(ipAddress);
        Socket thisSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        IPEndPoint theEndPoint = new IPEndPoint(target, sendPort);

        // Serializes data using protobufs, but protobuf not working here.
        byte[] sendBytes = Sender.SerializeData(sendObject);
        thisSocket.SendTo(sendBytes, theEndPoint);

        thisSocket.Close();
        listener.Close();


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
