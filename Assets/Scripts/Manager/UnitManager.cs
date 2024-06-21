using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    private static UnitManager _instance;

    public static UnitManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameObject("UnitManager").AddComponent<UnitManager>();
            }
            return _instance;
        }
    }

    Dictionary<int, Unit> _frinedUnitDic = new Dictionary<int, Unit>();
    Dictionary<int, Unit> _enemyUnitDic = new Dictionary<int, Unit>();

    public event Action<bool> _RetreatCallback;
    public event Action<bool> _CanelCallback;

    public void AddDictionaryUnit(Unit unit, bool isTagAlly)
    {
        unit.GetInstanceID();
        if(isTagAlly)
        {
            _frinedUnitDic.Add(unit.GetInstanceID(), unit);
        }
        else
        {
            _enemyUnitDic.Add(unit.GetInstanceID(), unit);
        }

    }

    public void RegisterRetreatCallback(Action<bool> retreatCallback)
    {
        _RetreatCallback += retreatCallback;
    }

    public void RegisterCancelCallback(Action<bool> moveCallback)
    {
        _CanelCallback += moveCallback;
    }

    public void UnRegisterRetreatCallback(Action<bool> retreatCallback)
    {
        _RetreatCallback -= retreatCallback;
    }

    public void UnRegisterCancelCallback(Action<bool> moveCallback)
    {
        _CanelCallback -= moveCallback;
    }  

    public void OnCalled_Retreat(bool isAlly)
    {
        _RetreatCallback?.Invoke(isAlly);
    }

    public void OnCalled_Cancel(bool isAlly)
    {
        _CanelCallback?.Invoke(isAlly);
    }
}
