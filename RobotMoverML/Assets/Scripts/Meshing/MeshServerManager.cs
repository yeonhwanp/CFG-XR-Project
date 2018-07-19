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

// Need to combine

public class MeshClientUDP<T> where T : IMessage<T>, new()
{
    // Unlike the robot script, I'm not going to accept messages because... I know it's possible and it's annoying to debug
    // But, will log the amount of elements the MeshList is holding so that we can verify it's the same.
    public static void UDPSend(int sendPort, T sendObject)
    {
        int listenPort = 0;

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
        else if (sendPort == 1234)
        {
            listenPort = 4321;
        }

        UdpClient listener = new UdpClient(listenPort);

        using (MemoryStream ms = new MemoryStream())
        {
            // Writing to the stream 
            sendObject.WriteTo(ms);
            ms.Position = 0;
            Debug.Log("ok here: " + ms.ToArray().Length);
            thisSocket.SendTo(ms.ToArray(), theEndPoint);
        }
        thisSocket.Close();
        listener.Close();
        Debug.Log("Data sent!");
    }
}