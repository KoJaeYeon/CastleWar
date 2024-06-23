using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class UnitSpawner : MonoBehaviour
{
    GameObject unitPrefab; // ū ������
    public List<string> subPrefabAddresses; // ���� �������� �ּ� ����Ʈ (������ ���� �ּ�)
    private Dictionary<string, GameObject> cachedSubPrefabs = new Dictionary<string, GameObject>(); // ĳ�̿� ��ųʸ�

    SpawnManager spawnManager; 
    void Start()
    {
        unitPrefab = Resources.Load("Prefabs/Unit_Base") as GameObject;
        SpawnUnitWithSubPrefab(0);


        //// ���� ���� �� ���� ������ �ε�
        //for (int i = 0; i < 12; i++) // ���÷� 12���� ���� �������� �ε�
        //{
        //    int randomIndex = Random.Range(0, subPrefabAddresses.Count); // ������ �ε��� ����
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
        unit.GetComponent<Unit>().InitAwake();
    }
}
