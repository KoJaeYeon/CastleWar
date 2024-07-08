using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
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

    private Queue<Action> executeQueue = new Queue<Action>();

    private void Update()
    {
        lock (executeQueue)
        {
            while (executeQueue.Count > 0)
            {
                var action = executeQueue.Dequeue();
                action?.Invoke();
            }
        }
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

    public void OnClick_SceneLoad()
    {
        SceneManager.LoadScene(1);
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
                    int lengthPlayerId = stream.Read(bytesPlayerId, 0, 4);
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

                    HandleIncommingRequest(typeOfService, playerId, length, bytes);

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }

    // Handle incomming request
    private void HandleIncommingRequest(int typeOfService, int playerId, int payloadLength, byte[] bytes)
    {
        Debug.Log("=========================================");
        Debug.Log("Type of Service : " + typeOfService);
        Debug.Log("player Id      : " + playerId);
        Debug.Log("Payload Length  : " + payloadLength);
        switch (typeOfService)
        {
            case 0:
                SpawnHandler(playerId, payloadLength, bytes);
                break;
            case 1:
                AddSlotHandler(playerId, payloadLength, bytes);
                break;
            case 2:
                AddSlotHandler(playerId, payloadLength, bytes);
                break;
            case 3:
                AccountHandler(payloadLength, bytes);
                break;
        }
    }

    void EnqueueCommand(Action action)
    {
        lock (executeQueue)
        {
            executeQueue.Enqueue(action);
        }
    }

    // Handle Spawn Signal
    private void SpawnHandler(int playerId, int payloadLength, byte[] bytes)
    {
        Debug.Log("Execute Spawn Handler");
        int unitSlot = BitConverter.ToInt32(bytes, 0);
        int x_axis = BitConverter.ToInt32(bytes, 4);
        int z_axis = BitConverter.ToInt32(bytes, 8);
        Debug.Log("UnitSlot     : " + unitSlot);
        Debug.Log("X axis     : " + x_axis);
        Debug.Log("Y axis     : " + z_axis);


    }

    // Handle AddSlot Signal
    private void AddSlotHandler(int playerId, int payloadLength, byte[] bytes)
    {
        Debug.Log("Execute AddSlot Handler");
        int unitId = BitConverter.ToInt32(bytes, 0);
        int slotIndex = BitConverter.ToInt32(bytes, 4);
        Debug.Log("Unit Id     : " + unitId);
        Debug.Log("Slot Index     : " + slotIndex);

        if(_playerId.Equals(playerId))
        {

        }
        else
        {
            slotIndex += 6;
        }

        //수신 쓰레드가 아닌 메인 쓰레드에서 실행시켜야 함
        EnqueueCommand(() => ExecuteAddUnitSlot(slotIndex, unitId));
    }

    // Handle Command Signal
    private void CommandHandler(int playerId, int payloadLength, byte[] bytes)
    {
        Debug.Log("Execute Command Handler");
        int typeOfCommand = BitConverter.ToInt32(bytes, 0);
        Debug.Log("Command Type     : " + typeOfCommand);


    }

    // Handle Account Signal
    private void AccountHandler(int payloadLength, byte[] bytes)
    {
        Debug.Log("Execute Account Handler");
        int playerId = BitConverter.ToInt32(bytes, 0);
        Debug.Log("Id     : " + playerId);

        SetPlayerId(playerId);
    }


    public void RequestAddUnitSlot(int slotIndex, int unitId)
    {
        var packet = PacketManager.Instance.GetAddSlotPacket(unitId, _playerId, slotIndex);
        SendPacket(packet);
    }

    private void ExecuteAddUnitSlot(int slotIndex, int unitId)
    {
        SpawnManager.Instance.OnAdd_ObjectPoolingSlot(slotIndex, unitId);
    }
    void SetPlayerId(int playerId)
    {
        _playerId = playerId;
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
