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

public class ClientUDP<T> where T : IMessage<T>, new()
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
            sendObject.WriteTo(ms);
            byte[] sendArray = ms.ToArray();
            int Sendlength = sendArray.Length;
            List<byte> sendList = new List<byte>();

            for (int byteCount = 0; byteCount <= Sendlength; byteCount++)
            {
                if (byteCount / 9999 != 1)
                {
                    sendList.Add(sendArray[byteCount]);
                }
                else
                {
                    thisSocket.SendTo(sendList.ToArray(), theEndPoint); 
                    sendList = new List<byte>();
                }
            }
            //thisSocket.SendTo(ms.ToArray(), theEndPoint);
        }
        Debug.Log("Data sent!");
    }
}