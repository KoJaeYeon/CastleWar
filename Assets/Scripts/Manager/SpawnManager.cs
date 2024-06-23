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

    #region Addressible
    [SerializeField] GameObject unit_Base_Prefab;

    private string defaultPath = "Assets/Resources_moved/Prefabs/Model_{0}.prefab";

    private Dictionary<int, GameObject> cachedSubPrefabs = new Dictionary<int, GameObject>(); // 캐싱용 딕셔너리

    private Stack<GameObject> Stack_BaseUnit = new Stack<GameObject>(); // 베이스 유닛 담아주는 스택 풀

    private void Awake()
    {
        unit_Base_Prefab = Resources.Load("Prefabs/Unit_Base") as GameObject;
        GetCacheSubPrefabModel(0);
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
            return Instantiate(unit_Base_Prefab);
        }
    }
    GameObject GetCacheSubPrefabModel(int id)
    {
        if (cachedSubPrefabs.ContainsKey(id))
        {
            // 캐시된 에셋 사용
            GameObject subPrefab = Instantiate(cachedSubPrefabs[id]);
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
                    //로드된 프리팹을 딕셔너리에 추가
                    cachedSubPrefabs[id] = handle.Result;
                }
                else
                {
                    Debug.LogError("Failed to load sub-prefab.");
                }
            };
        }
        return cachedSubPrefabs[id];

    }

    #endregion

    #region ObjectPooling
    Stack<GameObject>[] StackSpawnUnitObject = new Stack<GameObject>[12];

    //카드를 등록할 때 실행하는 함수, 10초에 걸쳐 생성
    void ObjectPoolingSlot(int index, int id)
    {
        Stack<GameObject> poolStack = StackSpawnUnitObject[0];
        poolStack = new Stack<GameObject>();
        GameObject subPrefab = GetCacheSubPrefabModel(id);
        StartCoroutine(PoolingForTerm(poolStack, subPrefab));
    }

    IEnumerator PoolingForTerm(Stack<GameObject> poolStack, GameObject subPrefab)
    {
        for(int i = 0; i < 50; i++)
        {
            //베이스 프리팹 가져오기
            GameObject baseUnit = GetBasePrefab();
            //하위 프리팹 베이스 프리팹에 생성해주기
            GameObject mergedUnit = Instantiate(subPrefab,baseUnit.transform);

            //풀에 담아두기
            poolStack.Push(mergedUnit);
            yield return new WaitForSeconds(0.1f);
        }
    }
    #endregion
}
