using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class UnitSpawner : MonoBehaviour
{
    public GameObject unitPrefab; // ū ������
    public List<string> subPrefabAddresses; // ���� �������� �ּ� ����Ʈ

    private Dictionary<string, GameObject> cachedSubPrefabs = new Dictionary<string, GameObject>(); // ĳ�̿� ��ųʸ�

    void Start()
    {
        // ���� ���� �� ���� ������ �ε�
        SpawnUnitWithSubPrefab(0); // ���÷� ù ��° �ּ� ���
    }

    void SpawnUnitWithSubPrefab(int index)
    {
        if (index >= 0 && index < subPrefabAddresses.Count)
        {
            string address = subPrefabAddresses[index];

            if (cachedSubPrefabs.ContainsKey(address))
            {
                // ĳ�õ� ���� ���
                GameObject subPrefab = Instantiate(cachedSubPrefabs[address]);
                AttachSubPrefabToUnit(subPrefab);
            }
            else
            {
                // ������ �ε��ϰ� ĳ�ÿ� ����
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
        GameObject unit = Instantiate(unitPrefab); // ū ������ �ν��Ͻ�ȭ
        subPrefab.transform.SetParent(unit.transform); // ���� �������� ū �������� �ڽ����� ����

        // �ʿ��� �ʱ�ȭ �۾� ����
    }
}
