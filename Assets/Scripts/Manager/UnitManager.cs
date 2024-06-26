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

    event Action<bool> _RetreatCallback;
    event Action<bool> _CanelCallback;

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
    public void RegisterRetreatCallback(Action<bool> retreatCallback)
    {
        _RetreatCallback += retreatCallback;
    }

    //유닛이 후퇴버튼이 눌렸을 때
    public void RegisterCancelCallback(Action<bool> moveCallback)
    {
        _CanelCallback += moveCallback;
    }

    //유닛이 죽을 때 또는 [TODO] 유닛이 복귀할 때
    public void UnRegisterRetreatCallback(Action<bool> retreatCallback)
    {
        _RetreatCallback -= retreatCallback;
    }

    //유닛이 취소버튼이 눌렸을 때
    public void UnRegisterCancelCallback(Action<bool> moveCallback)
    {
        _CanelCallback -= moveCallback;
        if(_CanelCallback == null)
        {
            Debug.Log("ReteratEnd");
            ChangeCancelButton.Invoke();
        }
    }  

    public void ChangeCancelButtonCallback(Action changeCancelButtonCallback)
    {
        ChangeCancelButton = changeCancelButtonCallback;
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
