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

    #region Addressible
    [SerializeField] GameObject unit_Base_Prefab;

    private string defaultPath = "Assets/Resources_moved/Prefabs/Model_{0}.prefab";

    private Stack<GameObject> Stack_BaseUnit = new Stack<GameObject>(); // 베이스 유닛 담아주는 스택 풀

    private Dictionary<int, GameObject> cachedSubPrefabs = new Dictionary<int, GameObject>(); // 캐싱용 딕셔너리

    private void Awake()
    {
        unit_Base_Prefab = Resources.Load("Prefabs/Unit_Base") as GameObject;

        _root = new GameObject("UnitPrefabRoot").transform;

        GameObject baseRoot = new GameObject("BaseRoot");
        baseRoot.transform.SetParent(_root);

        for (int i = 0; i < 200; i++)
        {
            GameObject prefab = Instantiate(unit_Base_Prefab, baseRoot.transform);
            prefab.SetActive(false);
            Stack_BaseUnit.Push(prefab);
        }
    }

    public void OnClick_AddSlot1()
    {        
        ObjectPoolingSlot(0,0);
    }

    public void Update()
    {
        if(Input.GetKeyUp(KeyCode.Escape))
        {
            Debug.Log("escape");
            foreach (GameObject cachedSubPrefabs in cachedSubPrefabs.Values)
            {
                Debug.Log(cachedSubPrefabs);
            }
        }
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

    //카드를 등록할 때 실행하는 함수, 10초에 걸쳐 생성
    void ObjectPoolingSlot(int index, int id)
    {
        Stack<GameObject> poolStack = StackSpawnUnitObject[index];
        poolStack = new Stack<GameObject>();
        GameObject slot = new GameObject($"slot[{index}]");
        slot.transform.SetParent(_root);
        GetCacheSubPrefabModel(id, subPrefab =>
        {
            StartCoroutine(PoolingForTerm(poolStack, subPrefab, slot.transform));
        });
    }

    IEnumerator PoolingForTerm(Stack<GameObject> poolStack, GameObject subPrefab, Transform root)
    {
        for(int i = 0; i < 50; i++)
        {
            //베이스 프리팹 가져오기
            GameObject baseUnit = GetBasePrefab();
            //하위 프리팹 베이스 프리팹에 생성해주기
            GameObject subUnitModel = Instantiate(subPrefab, baseUnit.transform);

            //풀에 담아두기
            poolStack.Push(baseUnit);

            //루트 위치 옮겨주기
            baseUnit.transform.SetParent(root);
            yield return new WaitForSeconds(0.1f);
        }
    }
    #endregion
}
