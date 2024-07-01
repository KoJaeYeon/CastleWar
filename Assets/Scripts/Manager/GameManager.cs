using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameObject("GameManager").AddComponent<GameManager>();
            }
            return _instance;
        }

    }



    private void Awake()
    {
        if (_instance == null || _instance == this)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public int _mana = 0;

    private Action<int> _manaChangeCallback;
    void Start()
    {
        Application.targetFrameRate = 60;
    }

    private void Update()
    {
        _mana += 1;
        _manaChangeCallback.Invoke(_mana);
    }

    public void RefreshManaInfo(int requestId, Action<int> callback)
    {
        callback.Invoke(_mana);
    }
    public void RegisterManaChangeCallback(Action<int> manaChangeCallback)
    {
        _manaChangeCallback += manaChangeCallback;
    }

    public void UnRegisterManaChangeCallback(Action<int> manaChangeCallback)
    {
        _manaChangeCallback -= manaChangeCallback;
    }
}
