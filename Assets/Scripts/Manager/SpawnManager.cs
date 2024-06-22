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

    private Dictionary<int, GameObject> cachedSubPrefabs = new Dictionary<int, GameObject>(); // ĳ�̿� ��ųʸ�

    private Stack<GameObject> Stack_BaseUnit = new Stack<GameObject>(); // ���̽� ���� ����ִ� ���� Ǯ

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
        //        // ĳ�õ� ���� ���
        //        GameObject subPrefab = Instantiate(cachedSubPrefabs[address]);
        //        AttachSubPrefabToUnit(subPrefab);
        //    }
        //    else
        //    {
        //        // ������ �ε��ϰ� ĳ�ÿ� ����
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
    //    GameObject unit = Instantiate(unitPrefab); // ū ������ �ν��Ͻ�ȭ
    //    subPrefab.transform.SetParent(unit.transform); // ���� �������� ū �������� �ڽ����� ����

    //    // �ʿ��� �ʱ�ȭ �۾� ����
    //    unit.GetComponent<Unit>().InitAwake();
    //}
}
