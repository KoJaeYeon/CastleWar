using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class UnitSpawner : MonoBehaviour
{
    GameObject unitPrefab; // 큰 프리팹
    public List<string> subPrefabAddresses; // 하위 프리팹의 주소 리스트 (각각의 개별 주소)
    private Dictionary<string, GameObject> cachedSubPrefabs = new Dictionary<string, GameObject>(); // 캐싱용 딕셔너리

    SpawnManager spawnManager; 
    void Start()
    {
        unitPrefab = Resources.Load("Prefabs/Unit_Base") as GameObject;
        SpawnUnitWithSubPrefab(0);


        //// 유닛 생성 및 하위 프리팹 로드
        //for (int i = 0; i < 12; i++) // 예시로 12개의 하위 프리팹을 로드
        //{
        //    int randomIndex = Random.Range(0, subPrefabAddresses.Count); // 무작위 인덱스 선택
        //    SpawnUnitWithSubPrefab(randomIndex);
        //}
    }

    public void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            Debug.Log("escape");
            foreach (GameObject cachedSubPrefabs in cachedSubPrefabs.Values)
            {
                Debug.Log(cachedSubPrefabs);
            }
        }
    }

    void SpawnUnitWithSubPrefab(int index)
    {
        if (index >= 0 && index < subPrefabAddresses.Count)
        {
            string address = "Assets/Resources_moved/Prefabs/Model_0.prefab";

            if (cachedSubPrefabs.ContainsKey(address))
            {
                // 캐시된 에셋 사용
                GameObject subPrefab = Instantiate(cachedSubPrefabs[address]);
                AttachSubPrefabToUnit(subPrefab);
            }
            else
            {
                // 에셋을 로드하고 캐시에 저장
                Addressables.LoadAssetAsync<GameObject>(address).Completed += handle =>
                {
                    if (handle.Status == AsyncOperationStatus.Succeeded)
                    {
                        cachedSubPrefabs[address] = handle.Result;
                        GameObject subPrefab = Instantiate(handle.Result);
                        AttachSubPrefabToUnit(subPrefab);
                    }
                    else
                    {
                        Debug.LogError("Failed to load sub-prefab.");
                    }
                };
            }
        }
    }

    void AttachSubPrefabToUnit(GameObject subPrefab)
    {
        GameObject unit = Instantiate(unitPrefab); // 큰 프리팹 인스턴스화
        subPrefab.transform.SetParent(unit.transform); // 하위 프리팹을 큰 프리팹의 자식으로 설정

        // 필요한 초기화 작업 수행
        unit.GetComponent<Unit>().InitAwake();
    }
}
