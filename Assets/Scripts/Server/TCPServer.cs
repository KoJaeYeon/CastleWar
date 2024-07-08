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

    static int playerId = 1;

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

                if (clientList.Count >= 2)
                {
                    keyValuePairs.Add(clientList[0], clientList[1]);
                    keyValuePairs.Add(clientList[1], clientList[0]);

                    clientList.RemoveRange(0, 2);
                    Console.WriteLine("Pair Searched");
                }
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

        while (true)
        {
            Byte[] bytesTypeOfService = new Byte[4];
            Byte[] bytesPlayerId = new Byte[4];
            Byte[] bytesPayloadLength = new Byte[4];

            int lengthTypeOfService = stream.Read(bytesTypeOfService, 0, 4);
            int lengthPlayerId = stream.Read(bytesPlayerId, 0, 4);
            int lengthPayloadLength = stream.Read(bytesPayloadLength, 0, 4);

            //연결이 끊어질 때 발생
            if (lengthTypeOfService <= 0 && lengthPlayerId <= 0 && lengthPayloadLength <= 0)
            {
                Console.WriteLine("Error!!!");
                break;
            }

            // Reverse byte order, in case of big endian architecture
            if (!BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytesTypeOfService);
                Array.Reverse(bytesPlayerId);
                Array.Reverse(bytesPayloadLength);
            }

            int typeOfService = BitConverter.ToInt32(bytesTypeOfService, 0);
            int playerId = BitConverter.ToInt32(bytesPlayerId, 0);
            int payloadLength = BitConverter.ToInt32(bytesPayloadLength, 0);

            Byte[] bytes = new Byte[payloadLength];
            int length = stream.Read(bytes, 0, payloadLength);
            Console.WriteLine(length);
            int packet = BitConverter.ToInt32(bytes, 0);
            Console.WriteLine(packet);

            // Create the resend packet
            Byte[] reSendPacket = new byte[12 + payloadLength];

            // Copy the typeOfService and payloadLength to the resend packet
            Array.Copy(bytesTypeOfService, 0, reSendPacket, 0, 4);
            Array.Copy(bytesPlayerId, 0, reSendPacket, 0, 4);
            Array.Copy(bytesPayloadLength, 0, reSendPacket, 4, 4);

            // Copy the payload to the resend packet
            Array.Copy(bytes, 0, reSendPacket, 12, payloadLength);

            if (typeOfService == 3)
            {
                // Set PlayerId
                bytes = BitConverter.GetBytes(playerId++);
                Array.Copy(bytes, 0, reSendPacket, 12, payloadLength);

                // Send the resend packet
                stream.Write(reSendPacket, 0, reSendPacket.Length);
            }
            else
            {
                // Send the resend packet
                stream.Write(reSendPacket, 0, reSendPacket.Length);

                // Send resend packet to pair
                var PairClient = keyValuePairs[client];
                var pairStream = PairClient.GetStream();
                pairStream.Write(reSendPacket, 0, reSendPacket.Length);

                Console.WriteLine("Resent packet with length: " + reSendPacket.Length);
            }




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
