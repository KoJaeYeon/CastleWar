using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private static UIManager _instance;

    public static UIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameObject("UIManager").AddComponent<UIManager>();
            }
            return _instance;
        }
    }

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            if (this != _instance)
            {
                Destroy(gameObject);
            }
        }
    }

    [SerializeField] GameObject PlayerPanel;
    [SerializeField] GameObject TopPanel;
    [SerializeField] ReceiptPanel ReceiptPanel;

    public void GameStart()
    {
        PlayerPanel.SetActive(true);
        TopPanel.SetActive(true);
    }

    public void GameEnd(bool isTagAlly)
    {
        PlayerPanel.SetActive(false);
        TopPanel.SetActive(false);
        ReceiptPanel.gameObject.SetActive(true);

        bool isTagAllyWin = !isTagAlly;
        ReceiptPanel.WhoIsWin(isTagAllyWin);
    }
}
