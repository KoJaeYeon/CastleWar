using System;
using System.Collections;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TcpSender : MonoBehaviour
{
    private static TcpSender _instance;

    public static TcpSender Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameObject("TcpSender").AddComponent<TcpSender>();
            }
            return _instance;
        }

    }

    private void Awake()
    {
        if (_instance == null || _instance == this)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Debug.Log(ConnectToServer());
    }

    TcpClient client;
    NetworkStream stream;
    [SerializeField] string server = "127.0.0.1";
    int port = 2074;
    bool isConnected = false;

    int _playerId;

    public void OnClick_SendPacket()
    {
        var packetManager = new PacketManager();
        SendPacket(packetManager.GetCommandPacket(2,2));
    }

    public void SendPacket(byte[] buffer)
    {
        Debug.Log("send");
        try
        {
            if (client == null)
            {
                Debug.Log("ConnectNeed");
                return;
            }

            // Get a client stream for reading and writing.
            stream = client.GetStream();

            // Send the message to the connected TcpServer.
            stream.Write(buffer, 0, buffer.Length);

            Console.WriteLine("Sent: {0}", buffer);
        }
        catch (ArgumentNullException e)
        {
            Console.WriteLine("ArgumentNullException: {0}", e);
        }
        catch (SocketException e)
        {
            Console.WriteLine("SocketException: {0}", e);
        }
    }

    public bool ConnectToServer()
    {
        if (isConnected) { return false; }
        try
        {
            client = new TcpClient(server, port);
            isConnected = true;
            stream = client.GetStream();

            // 수신 스레드 시작
            Thread receiveThread = new Thread(new ThreadStart(ReceiveData));
            receiveThread.IsBackground = true;
            receiveThread.Start();

            // 플레이어 Id 설정
            var packetManager = new PacketManager();
            SendPacket(packetManager.GetAccountPacket());

            return true;
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to connect to server: " + e.Message);
            return false;
        }
    }
    [SerializeField] TextMeshProUGUI temp;
    private void ReceiveData()
    {
        while (isConnected)
        {
            try
            {
                if (client.Available > 0)
                {
                    stream = client.GetStream();

                    Byte[] bytesTypeOfService = new Byte[4];
                    Byte[] bytesPlayerId = new Byte[4];
                    Byte[] bytesPayloadLength = new Byte[4];

                    int lengthTypeOfService = stream.Read(bytesTypeOfService, 0, 4);
                    int lengthPlayerId = stream.Read(bytesTypeOfService, 0, 4);
                    int lengthPayloadLength = stream.Read(bytesPayloadLength, 0, 4);

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




                    int packet = BitConverter.ToInt32(bytes, 0);
                    Debug.Log(packet);
                    temp.text += packet;
                    
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }

    void OnDestroy()
    {
        Disconnect();
    }

    public void Disconnect()
    {
        isConnected = false;

        if (stream != null)
            stream.Close();

        if (client != null)
            client.Close();
    }
}
