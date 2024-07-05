using System;
using System.Collections;
using System.Net.Sockets;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TcpSender : Singleton<TcpSender>
{
    TcpClient client;
    NetworkStream stream;
    public ChattingPanel chattingPanel;
    public TitleUI titleUI;
    [SerializeField] string server = "127.0.0.1";
    int port = 13000;
    bool isConnected = false;

    string myName = string.Empty;
    string enemyPlayerName = string.Empty;

    private void Awake()
    {
        if (Instance != this) Destroy(this.gameObject);
        DontDestroyOnLoad(this.gameObject);
    }

    public bool ConnectToServer()
    {
        if (isConnected) { return false; }
        try
        {
            client = new TcpClient(server, port);
            isConnected = true;
            stream = client.GetStream();

            // ���� ������ ����
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

    public void SendMyName()
    {
        SendMsg($"[���]/{myName}#");
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

                    // �����κ��� ������ �б�
                    byte[] buffer = new byte[client.Available];
                    stream.Read(buffer, 0, buffer.Length);

                    // ���� ������ ó��
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

            // 0.1�� ��� �� �ٽ� Ȯ��
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void ClassifyPacket(string receivedMessage)
    {
        if (receivedMessage.Contains("[��ȯ]"))
        {
            string[] packets = receivedMessage.Split('/');
            SpawnManager.Instance.Spawn_Troop(int.Parse(packets[1]), TroopType.Enemy);
        }
        else if(receivedMessage.Contains("[����]"))
        {
            titleUI.LoadScene();
        }
        else if(receivedMessage.Contains("[���]"))
        {
            string[] packets = receivedMessage.Split('/');
            OnSetEnemyName(packets[1]);
        }
        else
        {
            Debug.Log("Received: " + receivedMessage);
            if (string.IsNullOrWhiteSpace(receivedMessage)) return;
            chattingPanel.OnChatLogWrite($"<color=blue>{enemyPlayerName}</color> : {receivedMessage}\n");
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
