using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;

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
        SpawnUnitWithSubPrefab(0);
    }
    void SpawnUnitWithSubPrefab(int id)
    {

        string address = string.Format(defaultPath, "Warrior");
        Debug.Log(address);

        //    if (cachedSubPrefabs.ContainsKey(address))
        //    {
        //        // 캐시된 에셋 사용
        //        GameObject subPrefab = Instantiate(cachedSubPrefabs[address]);
        //        AttachSubPrefabToUnit(subPrefab);
        //    }
        //    else
        //    {
        //        // 에셋을 로드하고 캐시에 저장
        //        Addressables.LoadAssetAsync<GameObject>(address).Completed += handle =>
        //        {
        //            if (handle.Status == AsyncOperationStatus.Succeeded)
        //            {
        //                cachedSubPrefabs[address] = handle.Result;
        //                GameObject subPrefab = Instantiate(handle.Result);
        //                AttachSubPrefabToUnit(subPrefab);
        //            }
        //            else
        //            {
        //                Debug.LogError("Failed to load sub-prefab.");
        //            }
        //        };
        //    }
        //}
    }
    #endregion

    //void AttachSubPrefabToUnit(GameObject subPrefab)
    //{
    //    GameObject unit = Instantiate(unitPrefab); // 큰 프리팹 인스턴스화
    //    subPrefab.transform.SetParent(unit.transform); // 하위 프리팹을 큰 프리팹의 자식으로 설정

    //    // 필요한 초기화 작업 수행
    //    unit.GetComponent<Unit>().InitAwake();
    //}
}
