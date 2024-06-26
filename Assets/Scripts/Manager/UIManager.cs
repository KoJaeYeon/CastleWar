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

    PlayerPanel _playerPanel;

    public PlayerPanel PlayerPanel
    {
        get { return _playerPanel; }
        set { _playerPanel = value; }
    }
}
