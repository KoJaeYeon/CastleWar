using System.Collections.Generic;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class TCPServer
{
    Thread tcpListenerThread = null;
    List<TcpClient> clientList;
    Dictionary<TcpClient, TcpClient> keyValuePairs = new Dictionary<TcpClient, TcpClient>();
    TcpListener server;
    bool isServerRunning = true;

    Dictionary<TcpClient, string> playerName = new Dictionary<TcpClient, string>();

    static void Main(string[] args)
    {
        TCPServer server = new TCPServer();
        server.ServerStart();
        while (true)
        {
            string order = Console.ReadLine();
            Console.WriteLine($"order : {order}");
        }
    }

    private void Start()
    {
        Console.WriteLine("ServerStart");


        tcpListenerThread = new Thread(new ThreadStart(ServerStart));
        tcpListenerThread.IsBackground = true;
        tcpListenerThread.Start();
    }

    private void ServerStart()
    {
        clientList = new List<TcpClient>();
        server = null;
        try
        {
            Int32 port = 2074;
            IPAddress localAddr = IPAddress.Any;

            // TcpListener server = new TcpListener(port);
            server = new TcpListener(localAddr, port);
            // Start listening for client requests.
            server.Start();

            Console.WriteLine("Waiting for a connection... ");

            while (isServerRunning)
            {
                // Accept the TcpClient connection.
                TcpClient client = server.AcceptTcpClient();
                Console.WriteLine("Connect");
                // Start a new thread to handle communication with the client.
                Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClientCommunication));
                clientThread.Start(client);

                //Client 저장
                clientList.Add(client);

                //if (clientList.Count >= 2)
                //{
                //    keyValuePairs.Add(clientList[0], clientList[1]);
                //    keyValuePairs.Add(clientList[1], clientList[0]);

                //    clientList.RemoveRange(0, 2);
                //}
            }
        }
        catch (SocketException e)
        {
            Console.WriteLine("SocketException: " + e);
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

        int i;
        while (true)
        {
            Byte[] bytesTypeOfService = new Byte[4];
            Byte[] bytesPayloadLength = new Byte[4];

            int lengthTypeOfService = stream.Read(bytesTypeOfService, 0, 4);
            int lengthPayloadLength = stream.Read(bytesPayloadLength, 0, 4);

            //if (lengthTypeOfService <= 0 && lengthPayloadLength <= 0)
            //{
            //    break;
            //}

            // Reverse byte order, in case of big endian architecture
            if (!BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytesTypeOfService);
                Array.Reverse(bytesPayloadLength);
            }

            int typeOfService = BitConverter.ToInt32(bytesTypeOfService, 0);
            int payloadLength = BitConverter.ToInt32(bytesPayloadLength, 0);

            Byte[] bytes = new Byte[payloadLength];
            int length = stream.Read(bytes, 0, payloadLength);

            int packet = BitConverter.ToInt32(bytes, 0);
            Console.WriteLine(packet);

            //// Translate data bytes to a ASCII string.
            //data = System.Text.Encoding.UTF8.GetString(bytes, 0, i);
            //Console.WriteLine("Received: " + data);

            //if (data.Contains("[이름]"))
            //{
            //    playerName.Add(client, data.Split("#")[1]);
            //}

            //byte[] msg = System.Text.Encoding.UTF8.GetBytes(data);

            //NetworkStream enemyStream = keyValuePairs[client].GetStream();
            //enemyStream.Write(msg, 0, msg.Length);

            ////stream.Write(msg,0, msg.Length);
            //Console.WriteLine("Sent: " + data);
        }

        // Shutdown and close the client connection.
        client.Close();
        clientList.Remove(client);

        Console.WriteLine("Disconnect");
    }

    public void ServerEnd()
    {
        isServerRunning = false;
        server.Stop();
    }
}
