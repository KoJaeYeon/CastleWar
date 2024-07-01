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
    public int _mana = 75;
    public int _population = 0;
    public int _maxPopulation = 10;

    private Action<int> _manaChangeCallback;
    private Action<int,int> _populationChangeCallback;

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

    public void RefreshManaInfo(Action<int> callback)
    {
        callback.Invoke(_mana);
    }
    public void RefreshPopulationInfo(Action<int,int> callback)
    {
        callback.Invoke(_population,_maxPopulation);
    }


    public void RegisterManaChangeCallback(Action<int> manaChangeCallback)
    {
        _manaChangeCallback += manaChangeCallback;
    }

    public void UnRegisterManaChangeCallback(Action<int> manaChangeCallback)
    {
        _manaChangeCallback -= manaChangeCallback;
    }

    public void RegisterPopulationChangeCallback(Action<int, int> populationCallback)
    {
        _populationChangeCallback += populationCallback;
    }

    public void UnRegisterPopulationChangeCallback(Action<int, int> populationCallback)
    {
        _populationChangeCallback -= populationCallback;
    }

    public void RequestManaProduce(int mana)
    {
        _mana += mana;
        _manaChangeCallback.Invoke(_mana);
    }

    public bool RequestManaCheck(int mana)
    {
        if(_mana <= mana )
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public void RequestManaUse(int mana)
    {
        _mana += mana;
        _manaChangeCallback.Invoke(_mana);
    }

    public bool RequestPopulationCheck(int population)
    {
        if (_population + population > _maxPopulation)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    public void RequestPopulationImprove(int population)
    {
        _maxPopulation += population;
        _populationChangeCallback.Invoke(_population, _maxPopulation);
    }

    public void RequestPopulationUse(int population)
    {
        _population += population;
        _populationChangeCallback.Invoke(_population, _maxPopulation);
    }
}
