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

    Action _RetreatAllyCallback;
    Action _RetreatEnemyCallback;

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

    public void RegisterRetreatCallback(bool isAlly, Action retreatCallback)
    {
        if (isAlly)
        {
            _RetreatAllyCallback += retreatCallback;
        }
        else
        {
            _RetreatEnemyCallback += retreatCallback;
        }
    }

    public void UnRegisterRetreatCallback(bool isAlly, Action retreatCallback)
    {
        if (isAlly)
        {
            _RetreatAllyCallback -= retreatCallback;
        }
        else
        {
            _RetreatEnemyCallback -= retreatCallback;
        }
    }

    public void OnCalled_Retreat(bool isAlly)
    {
        if(isAlly)
        {
            _RetreatAllyCallback?.Invoke();
        }
        else
        {
            _RetreatEnemyCallback?.Invoke();
        }
    }

    public void ReturnUnitsToCastle(bool isTagAlly)
    {
        Dictionary<int, Unit> tempDic;
        tempDic = isTagAlly ? _frinedUnitDic : _enemyUnitDic;
        foreach(Unit unit in tempDic.Values)
        {
            unit.OnChange_RetreatState();
        }
    }
}
