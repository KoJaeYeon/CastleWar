using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class UnitSpawner : MonoBehaviour
{
    public GameObject unitPrefab; // 큰 프리팹
    public List<string> subPrefabAddresses; // 하위 프리팹의 주소 리스트

    void Start()
    {
        // 유닛 생성 및 하위 프리팹 로드
        SpawnUnitWithSubPrefab(0); // 예시로 첫 번째 주소 사용
    }

    void SpawnUnitWithSubPrefab(int index)
    {
        if (index >= 0 && index < subPrefabAddresses.Count)
        {
            Addressables.LoadAssetAsync<GameObject>(subPrefabAddresses[index]).Completed += OnSubPrefabLoaded;
        }
    }

    void OnSubPrefabLoaded(AsyncOperationHandle<GameObject> obj)
    {
        if (obj.Status == AsyncOperationStatus.Succeeded)
        {
            GameObject unit = Instantiate(unitPrefab); // 큰 프리팹 인스턴스화
            GameObject subPrefab = Instantiate(obj.Result); // 하위 프리팹 인스턴스화

            // 하위 프리팹을 큰 프리팹의 자식으로 설정
            subPrefab.transform.SetParent(unit.transform);

            // 필요한 초기화 작업 수행
        }
        else
        {
            Debug.LogError("Failed to load sub-prefab.");
        }
    }
}
