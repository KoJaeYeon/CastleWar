using System;
using System.Collections;
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
        ObjectPooling_Sanctuary_OnAwake();
        ObjectPooling_Camp_OnAwake();
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
    Stack<GameObject>[] StackSpawnUnitObject = new Stack<GameObject>[16];
    Dictionary<int, GameObject> mergedPrefab = new Dictionary<int, GameObject>();
    //Stack<GameObject> StackCampObject = new Stack<GameObject>();
    //Stack<GameObject> StackSanctuaryObject = new Stack<GameObject>();

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
    void ObjectPooling_Camp_OnAwake()
    {
        int campIdx = 6;
        StackSpawnUnitObject[campIdx] = new Stack<GameObject>();
        StackSpawnUnitObject[campIdx+8] = new Stack<GameObject>();

        GameObject campPrefab = Resources.Load("Prefabs/Camp") as GameObject;
        GameObject EcampPrefab = Resources.Load("Prefabs/Camp_E") as GameObject;
        mergedPrefab.Add(campIdx, campPrefab);
        mergedPrefab.Add(campIdx+8, EcampPrefab);
        GameObject campRoot = new GameObject("CampRoot");
        campRoot.transform.SetParent(_root);

        for(int i = 0; i < 30; i++)
        {
            //Ally Camp
            GameObject campPrefabClone = Instantiate(campPrefab, campRoot.transform);
            campPrefabClone.SetActive(false);

            //데이터 초기화
            var spawnUnit = campPrefabClone.GetComponent<Unit>();
            UnitData unitData = DatabaseManager.Instance.OnGetUnitData(-1);
            spawnUnit.InitData(unitData,campIdx);
            campPrefabClone.layer = LayerMask.NameToLayer("AllyBuilding");
            campPrefabClone.name = $"Camp_{i}";
            StackSpawnUnitObject[6].Push(campPrefabClone);

            //Enemy Camp
            GameObject EcampPrefabClone = Instantiate(EcampPrefab, campRoot.transform);
            EcampPrefabClone.SetActive(false);

            //데이터 초기화
            var EspawnUnit = EcampPrefabClone.GetComponent<Unit>();
            EspawnUnit.InitData(unitData,campIdx+8);
            EcampPrefabClone.layer = LayerMask.NameToLayer("EnemyBuilding");
            EcampPrefabClone.name = $"ECamp_{i}";
            StackSpawnUnitObject[campIdx+8].Push(EcampPrefabClone);
        }

    }

    void ObjectPooling_Sanctuary_OnAwake()
    {
        int sacnIdx = 7;
        StackSpawnUnitObject[sacnIdx] = new Stack<GameObject>();
        StackSpawnUnitObject[sacnIdx+8] = new Stack<GameObject>();

        GameObject sanctuaryPrefab = Resources.Load("Prefabs/Sanctuary") as GameObject;
        mergedPrefab.Add(sacnIdx, sanctuaryPrefab);
        GameObject sanctuaryRoot = new GameObject("Sanctuary");
        sanctuaryRoot.transform.SetParent(_root);

        for (int i = 0; i < 10; i++)
        {
            //Ally Sanc
            GameObject sancPrefabClone = Instantiate(sanctuaryPrefab, sanctuaryRoot.transform);
            sancPrefabClone.SetActive(false);

            //데이터 초기화
            var spawnUnit = sancPrefabClone.GetComponent<Unit>();
            UnitData unitData = DatabaseManager.Instance.OnGetUnitData(-2);
            spawnUnit.InitData(unitData,sacnIdx);
            sancPrefabClone.layer = LayerMask.NameToLayer("AllyBuilding");
            sancPrefabClone.name = $"Sanctuary_{i}";
            StackSpawnUnitObject[sacnIdx].Push(sancPrefabClone);

            //Enemy Sanc
            GameObject EancPrefabClone = Instantiate(sanctuaryPrefab, sanctuaryRoot.transform);
            EancPrefabClone.SetActive(false);
            //데이터 초기화
            var EspawnUnit = EancPrefabClone.GetComponent<Unit>();
            EspawnUnit.InitData(unitData,sacnIdx+8);
            EancPrefabClone.layer = LayerMask.NameToLayer("EnemyBuilding");
            EancPrefabClone.name = $"ESanctuary_{i}";
            StackSpawnUnitObject[sacnIdx+8].Push(EancPrefabClone);
        }

    }

    //카드를 등록할 때 실행하는 함수, 9초에 걸쳐 생성
    public void OnAdd_ObjectPoolingSlot(int index, int id) // 0 ~ 5 : Ally, 6 ~ 7 Camp,Sanctuary// 8 ~ 13 : Enemy //14 15 Camp,Sanc
    {
        StackSpawnUnitObject[index] = new Stack<GameObject>();
        Stack<GameObject> poolStack = StackSpawnUnitObject[index];
        GameObject slot = new GameObject($"slot[{index}]");
        slot.transform.SetParent(_root);

        //subPrefab의 비동기 작업이 완료된 후 콜백
        GetCacheSubPrefabModel(id, subPrefab =>
        {
            StartCoroutine(PoolingForTerm(poolStack, subPrefab, slot.transform, id, index));            
        });

    }

    IEnumerator PoolingForTerm(Stack<GameObject> poolStack, GameObject subPrefab, Transform root, int id, int index)
    {
        //베이스 프리팹 가져오기
        GameObject baseUnit = GetBasePrefab();

        //하위 프리팹 베이스 프리팹에 생성해주기
        GameObject subUnitModel = Instantiate(subPrefab, baseUnit.transform);
        

        //초기 데이터 부여
        var unit = baseUnit.GetComponent<Unit>();
        UnitData unitData = DatabaseManager.Instance.OnGetUnitData(id);
        unit.InitData(unitData, index);
        if (index < 8)
        {
            baseUnit.tag = "Ally";
            switch (unitData.unitType)
            {
                case UnitType.Ground:
                    baseUnit.layer = LayerMask.NameToLayer("AllyGroundUnit");
                    break;
                case UnitType.Air:
                    baseUnit.layer = LayerMask.NameToLayer("AllyAirUnit");
                    break;
                case UnitType.Building:
                    baseUnit.layer = LayerMask.NameToLayer("AllyBuilding");
                    break;
            }
        }
        else
        {
            baseUnit.tag = "Enemy";
            subUnitModel.transform.Rotate(0, 180, 0);
            switch (unitData.unitType)
            {
                case UnitType.Ground:
                    baseUnit.layer = LayerMask.NameToLayer("EnemyGroundUnit");
                    break;
                case UnitType.Air:
                    baseUnit.layer = LayerMask.NameToLayer("EnemyAirUnit");
                    break;
                case UnitType.Building:
                    baseUnit.layer = LayerMask.NameToLayer("EnemyBuilding");
                    break;
            }
        }
        //숫자가 부족하면 추가생성을 위한 프리팹 저장
        mergedPrefab.Add(index, baseUnit);

        //만들어진 프리팹을 복사해서 풀에 저장
        for (int i = 0; i < 30; i++)
        {
            //프리팹 복사
            var SpawnPrefab = Instantiate(baseUnit, root);

            //데이터 초기화
            var spawnUnit = SpawnPrefab.GetComponent<Unit>();
            spawnUnit.InitData(unitData, index);
            SpawnPrefab.name = $"{unitData.name}_{i}";

            //풀에 담아두기
            poolStack.Push(SpawnPrefab);

            yield return new WaitForSeconds(0.3f);
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

            //데이터 초기화
            Unit newPrefabUnit = newPrefab.GetComponent<Unit>();
            UnitData unitData = DatabaseManager.Instance.OnGetUnitData(newPrefabUnit.UnitId);
            newPrefabUnit.InitData(unitData, index);

            //반환
            newPrefab.SetActive(false);
            return newPrefab;
        }
    }

    public void OnCalled_ReturnUnit(int index, GameObject returnUnit) //유닛 복귀할때 또는 죽을 때
    {
        returnUnit.SetActive(false);
        StackSpawnUnitObject[index].Push(returnUnit);
    }

    //public GameObject OnCalled_GetCamp()
    //{
    //    if (StackCampObject.TryPop(out GameObject result))
    //    {
    //        return result;
    //    }
    //    else
    //    {
    //        GameObject newPrefab = Instantiate(mergedPrefab[-1]);
    //        newPrefab.SetActive(false);
    //        return newPrefab;
    //    }
    //}

    //public GameObject OnCalled_GetSanctuary()
    //{
    //    if (StackSanctuaryObject.TryPop(out GameObject result))
    //    {
    //        return result;
    //    }
    //    else
    //    {
    //        GameObject newPrefab = Instantiate(mergedPrefab[-2]);
    //        newPrefab.SetActive(false);
    //        return newPrefab;
    //    }
    //}

    //public void OnCalled_ReturnCamp(GameObject returnUnit) //막사가 파괴될 때
    //{
    //    returnUnit.SetActive(false);
    //    StackCampObject.Push(returnUnit);
    //}

    //public void OnCalled_ReturnSanc(GameObject returnUnit) //막사가 파괴될 때
    //{
    //    returnUnit.SetActive(false);
    //    StackSanctuaryObject.Push(returnUnit);
    //}
}
