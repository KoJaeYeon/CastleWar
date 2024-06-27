using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

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

    event Action _AllyRetreatCallback;
    event Action _AllyCancelCallback;
    event Action _EnemyRetreatCallback;
    event Action _EnemyCanceltCallback;

    //event Action<bool> _RetreatCallback;
    //event Action<bool> _CanelCallback;

    event Action ChangeCancelButton;

    public void AddDictionaryUnit(Unit unit, bool isTagAlly)
    {
        unit.GetInstanceID();
        if (isTagAlly)
        {
            _frinedUnitDic.Add(unit.GetInstanceID(), unit);
        }
        else
        {
            _enemyUnitDic.Add(unit.GetInstanceID(), unit);
        }

    }

    //유닛 움직이기 시작할 때
    public void RegisterRetreatCallback(bool isTagAlly, Action retreatCallback)
    {
        if(isTagAlly)
        {
            _AllyRetreatCallback += retreatCallback;
        }
        else
        {
            _EnemyRetreatCallback += retreatCallback;
        }
        
    }

    //유닛이 후퇴버튼이 눌렸을 때
    public void RegisterCancelCallback(bool isTagAlly, Action moveCallback)
    {
        if (isTagAlly)
        {
            _AllyCancelCallback += moveCallback;
        }
        else
        {
            _EnemyRetreatCallback += moveCallback;
        }

    }

    //유닛이 죽을 때 또는 [TODO] 유닛이 복귀할 때
    public void UnRegisterRetreatCallback(bool isTagAlly, Action retreatCallback)
    {
        if (isTagAlly)
        {
            _AllyRetreatCallback -= retreatCallback;
        }
        else
        {
            _EnemyRetreatCallback -= retreatCallback;
        }
    }

    //유닛이 취소버튼이 눌렸을 때
    public void UnRegisterCancelCallback(bool isTagAlly, Action moveCallback)
    {
        if (isTagAlly)
        {
            _AllyCancelCallback -= moveCallback;
        }
        else
        {
            _EnemyRetreatCallback -= moveCallback;
        }
        if(_AllyCancelCallback == null)
        {
            Debug.Log("ReteratEnd");
            ChangeCancelButton.Invoke();
        }
    }  

    public void ChangeCancelButtonCallback(Action changeCancelButtonCallback)
    {
        ChangeCancelButton = changeCancelButtonCallback;
    }
    public void OnCalled_Retreat(bool isTagAlly)
    {
        if (isTagAlly)
        {
            _AllyRetreatCallback?.Invoke();
        }
        else
        {
            _EnemyRetreatCallback?.Invoke();
        }
    }

    public void OnCalled_Cancel(bool isTagAlly)
    {
        if (isTagAlly)
        {
            _AllyCancelCallback?.Invoke();
        }
        else
        {
            _EnemyCanceltCallback?.Invoke();
        }
    }


}
