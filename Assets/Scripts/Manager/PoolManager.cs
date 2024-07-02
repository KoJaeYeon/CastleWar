using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    private static PoolManager _instance;

    public static PoolManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameObject("PoolManager").AddComponent<PoolManager>();
            }
            return _instance;
        }

    }
    private void Awake()
    {
        if (_instance == null || _instance == this)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        for(int i = 0; i < 12; i++)
        {
            projectileStacks[i] = new Stack<GameObject>();
        }
    }

    Dictionary<int, GameObject> projectilePrefabDic = new Dictionary<int, GameObject>();
    Stack<GameObject>[] projectileStacks = new Stack<GameObject>[12];

    public Dictionary<int, GameObject> ProjectilePrefabDic
    {
        get { return projectilePrefabDic; }
    }

    public void AddKey_ProjectilePrefabDic()
    {

    }


    public GameObject GetPrefab(int slotKey, int unitId)
    {
        if (projectileStacks[slotKey].TryPop(out GameObject prefab))
        {
            return prefab;
        }
        else
        {
            if(projectilePrefabDic.ContainsKey(slotKey))
            {
                return Instantiate(projectilePrefabDic[slotKey]);
            }
            else
            {
                GameObject orginalPrefab = Resources.Load<GameObject>($"Prefabs/Projectile/Projectile_{unitId}");
                projectilePrefabDic.Add(slotKey, orginalPrefab);
                return Instantiate(orginalPrefab);
            }
        }
    }

    public void ReturnPrefab(int slotKey, GameObject projectile)
    {
        projectileStacks[slotKey].Push(projectile);
    }
}
