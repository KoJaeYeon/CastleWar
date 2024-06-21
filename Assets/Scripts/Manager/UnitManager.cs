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

    #region Addresible

    List<string> subPrefabAddresses = new List<string>(); // 하위 프리팹의 주소 리스트
    void SpawnUnitWithSubPrefab(int index)
    {
        subPrefabAddresses.Add("Assets/Resources_moved/Prefabs/Warrior/Model_Warrior.prefab");
        if (index >= 0 && index < subPrefabAddresses.Count)
        {
            Addressables.LoadAssetAsync<GameObject>(subPrefabAddresses[index]).Completed += OnSubPrefabLoaded;
        }
    }

    void OnSubPrefabLoaded(AsyncOperationHandle<GameObject> obj)
    {
        if (obj.Status == AsyncOperationStatus.Succeeded)
        {
            GameObject subPrefab = Instantiate(obj.Result); // 하위 프리팹 인스턴스화

            // 하위 프리팹을 큰 프리팹의 자식으로 설정
            subPrefab.transform.SetParent(transform);

            // 필요한 초기화 작업 수행
        }
        else
        {
            Debug.LogError("Failed to load sub-prefab.");
        }
    }
    #endregion
}
