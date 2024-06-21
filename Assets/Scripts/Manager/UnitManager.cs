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

    Action _RetreatFriendCallback;
    Action _RetreatEnemyCallback;

    public void AddDictionaryUnit(Unit unit, bool isTagFriend)
    {
        unit.GetInstanceID();
        if(isTagFriend)
        {
            _frinedUnitDic.Add(unit.GetInstanceID(), unit);
        }
        else
        {
            _enemyUnitDic.Add(unit.GetInstanceID(), unit);
        }

    }

    public void RegisterRetreatCallback(bool isFriend, Action retreatCallback)
    {
        if (isFriend)
        {
            _RetreatFriendCallback += retreatCallback;
        }
        else
        {
            _RetreatEnemyCallback += retreatCallback;
        }
    }

    public void UnRegisterRetreatCallback(bool isFriend, Action retreatCallback)
    {
        if (isFriend)
        {
            _RetreatFriendCallback -= retreatCallback;
        }
        else
        {
            _RetreatEnemyCallback -= retreatCallback;
        }
    }

    public void OnCalled_Retreat(bool isFriend)
    {
        if(isFriend)
        {
            _RetreatFriendCallback?.Invoke();
        }
        else
        {
            _RetreatEnemyCallback?.Invoke();
        }
    }

    public void ReturnUnitsToCastle(bool isTagFriend)
    {
        Dictionary<int, Unit> tempDic;
        tempDic = isTagFriend ? _frinedUnitDic : _enemyUnitDic;
        foreach(Unit unit in tempDic.Values)
        {
            unit.OnChange_RetreatState();
        }
    }
}
