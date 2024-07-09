using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleCanvas : MonoBehaviour
{
    [SerializeField] GameObject _cancelButton;
    private void OnEnable()
    {
        TcpSender.Instance.CancelButton = _cancelButton;
    }
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

    public void OnClick_CancelMatch()
    {
        TcpSender.Instance.Disconnect();
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
