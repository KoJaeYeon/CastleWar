using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleCanvas : MonoBehaviour
{
    public void OnClick_MatchGame()
    {
        Coroutine sendPing = TcpSender.Instance.SendPing;
        if (sendPing == null)
        {
            bool connected = TcpSender.Instance.ConnectToServer();
            if (connected)
            {
                sendPing = StartCoroutine(SendPing());
            }
        }
    }

    IEnumerator SendPing()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            TcpSender.Instance.RequestCommand(6);
        }
    }
}
