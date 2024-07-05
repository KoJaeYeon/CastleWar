using System;
using System.Collections;
using System.Net.Sockets;
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
        ConnectToServer();
    }

    TcpClient client;
    NetworkStream stream;
    [SerializeField] string server = "127.0.0.1";
    int port = 2074;
    bool isConnected = false;

    string myName = string.Empty;
    string enemyPlayerName = string.Empty;

    public void OnClick_SendPacket()
    {
        var packetManager = new PacketManager();
        SendPacket(packetManager.GetCommandPacket(2));
    }

    public void SendPacket(byte[] buffer)
    {
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
            StartCoroutine(ReceiveData());
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to connect to server: " + e.Message);
            return false;
        }
    }

    public void OnSetMyName(string myName)
    {
        this.myName = myName;
    }

    public void OnSetEnemyName(string enemyName)
    {
        enemyPlayerName = enemyName;
    }

    public string GetMyName()
    {
        return myName;
    }

    public void SendMsg(String message)
    {
        try
        {
            if (client == null)
            {
                Debug.Log("ConnectNeed");
                return;
            }
            // Translate the passed message into ASCII and store it as a Byte array.
            Byte[] data = System.Text.Encoding.UTF8.GetBytes($"{message}#");

            // Get a client stream for reading and writing.
            stream = client.GetStream();

            // Send the message to the connected TcpServer.
            stream.Write(data, 0, data.Length);

            Console.WriteLine("Sent: {0}", message);
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

    IEnumerator ReceiveData()
    {
        while (isConnected)
        {
            try
            {
                if (client.Available > 0)
                {
                    stream = client.GetStream();

                    // 서버로부터 데이터 읽기
                    byte[] buffer = new byte[client.Available];
                    stream.Read(buffer, 0, buffer.Length);

                    // 받은 데이터 처리
                    string receivedMessage = System.Text.Encoding.UTF8.GetString(buffer);

                    string[] packets = receivedMessage.Split("#");
                    foreach (string packet in packets)
                    {
                        ClassifyPacket(packet);
                    }
                    

                }
            }
            catch (Exception e)
            {
                Debug.LogError("Error receiving data: " + e.Message);
            }

            // 0.1초 대기 후 다시 확인
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void ClassifyPacket(string receivedMessage)
    {

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
