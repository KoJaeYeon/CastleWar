using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class UnitSpawner : MonoBehaviour
{
    public GameObject unitPrefab; // ū ������
    public List<string> subPrefabAddresses; // ���� �������� �ּ� ����Ʈ

    void Start()
    {
        // ���� ���� �� ���� ������ �ε�
        SpawnUnitWithSubPrefab(0); // ���÷� ù ��° �ּ� ���
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
            GameObject unit = Instantiate(unitPrefab); // ū ������ �ν��Ͻ�ȭ
            GameObject subPrefab = Instantiate(obj.Result); // ���� ������ �ν��Ͻ�ȭ

            // ���� �������� ū �������� �ڽ����� ����
            subPrefab.transform.SetParent(unit.transform);

            // �ʿ��� �ʱ�ȭ �۾� ����
        }
        else
        {
            Debug.LogError("Failed to load sub-prefab.");
        }
    }
}
