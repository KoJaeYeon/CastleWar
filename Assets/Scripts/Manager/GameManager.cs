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

    Coroutine manaCoroutine;
    void Start()
    {
        Application.targetFrameRate = 60;
        manaCoroutine = StartCoroutine(ProduceMana());
    }

    //게임 시작할때 캐슬 마나생산
    IEnumerator ProduceMana()
    {
        while(true)
        {
            yield return new WaitForSeconds(2f);
            _mana += 10;
            _manaChangeCallback.Invoke(_mana);
        }
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

    public void RequestManaProduce(int mana)
    {
        _mana += mana;
        _manaChangeCallback.Invoke(_mana);
    }

    public bool RequestManaUse(int mana)
    {
        if(_mana <= mana )
        {
            return false;
        }
        else
        {
            _mana -= mana;
            _manaChangeCallback.Invoke(_mana);
            return true;
        }
    }
}
