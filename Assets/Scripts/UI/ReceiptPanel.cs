using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReceiptPanel : MonoBehaviour
{
    [SerializeField] GameObject Victory;
    [SerializeField] GameObject Defeat;
    public void WhoIsWin(bool isTagAllyWin)
    {
        Victory.SetActive(isTagAllyWin);
        Defeat.SetActive(!isTagAllyWin);

    }
    public void OnClick_ExitGame()
    {
        Application.Quit();
    }

    public void OnClick_MainMenu()
    {
        TcpSender.Instance.Disconnect();
        SceneManager.LoadScene(0);
    }
}
