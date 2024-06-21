using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class UnitSpawner : MonoBehaviour
{
    public GameObject unitPrefab; // 큰 프리팹
    public List<string> subPrefabAddresses; // 하위 프리팹의 주소 리스트

    private Dictionary<string, GameObject> cachedSubPrefabs = new Dictionary<string, GameObject>(); // 캐싱용 딕셔너리

    void Start()
    {
        // 유닛 생성 및 하위 프리팹 로드
        SpawnUnitWithSubPrefab(0); // 예시로 첫 번째 주소 사용
    }

    void SpawnUnitWithSubPrefab(int index)
    {
        if (index >= 0 && index < subPrefabAddresses.Count)
        {
            string address = subPrefabAddresses[index];

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
    }
}
