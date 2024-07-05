using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class TCPServer : MonoBehaviour
{
    Thread tcpListenerThread;
    List<TcpClient> clientList;
    Dictionary<TcpClient, TcpClient> keyValuePairs = new Dictionary<TcpClient, TcpClient>();
    TcpListener server;
    bool isServerRunning = true;

    Dictionary<TcpClient, string> playerName = new Dictionary<TcpClient, string>();

    private void Start()
    {
        Debug.Log("ServerStart");
        clientList = new List<TcpClient>();

        tcpListenerThread = new Thread(new ThreadStart(ServerStart));
        tcpListenerThread.IsBackground = true;
        tcpListenerThread.Start();
    }

    private void ServerStart()
    {
        server = null;
        try
        {
            // Set the TcpListener on port 13000.
            Int32 port = 13000;
            IPAddress localAddr = IPAddress.Parse("127.0.0.1");

            // TcpListener server = new TcpListener(port);
            server = new TcpListener(localAddr, port);
            // Start listening for client requests.
            server.Start();

            Debug.Log("Waiting for a connection... ");

            while (isServerRunning)
            {
                // Accept the TcpClient connection.
                TcpClient client = server.AcceptTcpClient();
                Debug.Log("Connect");
                // Start a new thread to handle communication with the client.
                Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClientCommunication));
                clientThread.Start(client);

                //Client 저장
                clientList.Add(client);

                if (clientList.Count >= 2)
                {
                    keyValuePairs.Add(clientList[0], clientList[1]);
                    keyValuePairs.Add(clientList[1], clientList[0]);

                    clientList.RemoveRange(0, 2);
                }
            }
        }
        catch (SocketException e)
        {
            Debug.Log("SocketException: " + e);
        }
        finally
        {
            server.Stop();
        }
    }

    private void HandleClientCommunication(object clientObj)
    {
        TcpClient client = (TcpClient)clientObj;
        NetworkStream stream = client.GetStream();

        Byte[] bytes = new Byte[256];
        String data = null;

        int i;
        while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
        {
            // Translate data bytes to a ASCII string.
            data = System.Text.Encoding.UTF8.GetString(bytes, 0, i);
            Debug.Log("Received: " + data);

            if(data.Contains("[이름]"))
            {
                playerName.Add(client, data.Split("#")[1]);
            }

            byte[] msg = System.Text.Encoding.UTF8.GetBytes(data);

            NetworkStream enemyStream = keyValuePairs[client].GetStream();
            enemyStream.Write(msg, 0, msg.Length);

            //stream.Write(msg,0, msg.Length);
            Debug.Log("Sent: " + data);
        }

        // Shutdown and close the client connection.
        client.Close();
        clientList.Remove(client);
    }

    public void ServerEnd()
    {
        isServerRunning = false;
        server.Stop();
    }
}
