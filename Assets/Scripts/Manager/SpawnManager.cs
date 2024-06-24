﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
public class SpawnManager : MonoBehaviour
{
    private static SpawnManager _instance;
    public static SpawnManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameObject(nameof(SpawnManager)).AddComponent<SpawnManager>();
            }
            return _instance;
        }
    }

    Transform _root;
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        Load_BasePrefab_OnAwake();
        ObjectPooling_BasePrefab_OnAwake();
    }

    #region Addressible
    GameObject unit_Base_Prefab;

    private string defaultPath = "Assets/Resources_moved/Prefabs/Model_{0}.prefab";

    private Stack<GameObject> Stack_BaseUnit = new Stack<GameObject>(); // 베이스 유닛 담아주는 스택 풀

    private Dictionary<int, GameObject> cachedSubPrefabs = new Dictionary<int, GameObject>(); // 캐싱용 딕셔너리

    void Load_BasePrefab_OnAwake()
    {
        unit_Base_Prefab = Resources.Load("Prefabs/Unit_Base") as GameObject;

        _root = new GameObject("UnitPrefabRoot").transform;
    }

    //public void OnClick_AddSlot1()
    //{        
    //    ObjectPoolingSlot(0,0);
    //}

    //public void OnClick_AddSlot2()
    //{
    //    ObjectPoolingSlot(1, 1);
    //}

    //모델링 없는 기초 베이스모델 반환, 게임을 시작할 때 미리 생성
    GameObject GetBasePrefab()
    {
        if(Stack_BaseUnit.TryPop(out GameObject result))
        {
            return result;
        }
        else
        {
            Debug.Log("Instantiate");
            return Instantiate(unit_Base_Prefab);
        }
    }
    void GetCacheSubPrefabModel(int id, System.Action<GameObject> onLoaded)
    {
        if (cachedSubPrefabs.ContainsKey(id))
        {
            // 캐시된 에셋 사용
            onLoaded?.Invoke(cachedSubPrefabs[id]);
        }
        else
        {
            string address = string.Format(defaultPath, id);
            Debug.Log(address);
            // 에셋을 로드하고 캐시에 저장
            Addressables.LoadAssetAsync<GameObject>(address).Completed += handle =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    // 로드된 프리팹을 딕셔너리에 추가
                    cachedSubPrefabs[id] = handle.Result;
                    onLoaded?.Invoke(handle.Result);
                }
                else
                {
                    Debug.LogError("Failed to load sub-prefab.");
                }
            };
        }
    }

    #endregion
    #region ObjectPooling
    Stack<GameObject>[] StackSpawnUnitObject = new Stack<GameObject>[12];
    Dictionary<int, GameObject> mergedPrefab = new Dictionary<int, GameObject>();

    void ObjectPooling_BasePrefab_OnAwake()
    {
        GameObject baseRoot = new GameObject("BaseRoot");
        baseRoot.transform.SetParent(_root);

        for (int i = 0; i < 360; i++)
        {
            GameObject prefab = Instantiate(unit_Base_Prefab, baseRoot.transform);
            prefab.SetActive(false);
            Stack_BaseUnit.Push(prefab);
        }
    }

    //카드를 등록할 때 실행하는 함수, 10초에 걸쳐 생성
    public void OnAdd_ObjectPoolingSlot(int index, int id) // 0 ~ 5 : Ally, // 6 ~ 11 : Enemy
    {
        Stack<GameObject> poolStack = StackSpawnUnitObject[index];
        poolStack = new Stack<GameObject>();
        GameObject slot = new GameObject($"slot[{index}]");
        slot.transform.SetParent(_root);

        //subPrefab의 비동기 작업이 완료된 후 콜백
        GetCacheSubPrefabModel(id, subPrefab =>
        {
            StartCoroutine(PoolingForTerm(poolStack, subPrefab, slot.transform, index));
            //숫자가 부족하면 추가생성을 위한 프리팹 저장
            mergedPrefab.Add(index, Instantiate(poolStack.Peek(),slot.transform));
        });
    }

    IEnumerator PoolingForTerm(Stack<GameObject> poolStack, GameObject subPrefab, Transform root, int index)
    {
        for(int i = 0; i < 30; i++)
        {
            //베이스 프리팹 가져오기
            GameObject baseUnit = GetBasePrefab();
            //하위 프리팹 베이스 프리팹에 생성해주기
            GameObject subUnitModel = Instantiate(subPrefab, baseUnit.transform);
            baseUnit.GetComponent<Unit>().SetSpawnSlotIndex(index);

            //풀에 담아두기
            poolStack.Push(baseUnit);

            //루트 위치 옮겨주기
            baseUnit.transform.SetParent(root);
            yield return new WaitForSeconds(0.1f);
        }
    }
    #endregion

    public GameObject OnCalled_GetUnit(int index)
    {
        if(StackSpawnUnitObject[index].TryPop(out GameObject result))
        {
            return result;
        }
        else
        {
            GameObject newPrefab = Instantiate(mergedPrefab[index]);
            newPrefab.SetActive(false);
            return newPrefab;
        }
    }

    public void OnCalled_ReturnUnit(int index, GameObject returnUnit)
    {
        StackSpawnUnitObject[index].Push(returnUnit);
    }
}