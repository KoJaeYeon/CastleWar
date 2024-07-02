using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastleManager : MonoBehaviour
{
    private static CastleManager _instance;

    public static CastleManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameObject("CastleManager").AddComponent<CastleManager>();
            }
            return _instance;
        }

    }

    public void Awake()
    {
        if (_instance == null || _instance == this)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [SerializeField] GameObject AllyCastle;
    [SerializeField] GameObject EnemyCastle;
    [SerializeField] CircleUnionManager AllyUnion;
    [SerializeField] CircleUnionManager EnemyUnion;
    [SerializeField] Material _allyMaterial;
    [SerializeField] Material _enemyMaterial;

    public Material AllyMaterial => _allyMaterial;
    public Material EnemyMaterial => _enemyMaterial;




    public GameObject GetCastleGameObj(bool isFriend)
    {
        if(isFriend)
        {
            return AllyCastle;
        }
        else
        {
            return EnemyCastle;
        }
    }

    private void Start()
    {
        float castleRadius = 26f;
        AllyUnion.AddCircle(AllyCastle.transform, castleRadius);
        EnemyUnion.AddCircle(EnemyCastle.transform,castleRadius);

        Castle allyCastle = AllyCastle.GetComponent<Castle>();
        Castle enemyCastle = EnemyCastle.GetComponent<Castle>();
        var castleData = DatabaseManager.Instance.OnGetUnitData(-3);
        allyCastle.InitData(castleData);
        //enemyCastle.InitData(castleData);

        allyCastle.StartState();        
    }

    public void AddCampToUnion(Transform transform, bool isTagAlly)
    {
        if(isTagAlly)
        {
            AllyUnion.AddCircle(transform);
        }
        else
        {
            EnemyUnion.AddCircle(transform);
        }
    }

    public void AddSancToUnion(Transform transform, bool isTagAlly)
    {
        float sancRadius = 6f;
        if (isTagAlly)
        {
            AllyUnion.AddCircle(transform, sancRadius);
        }
        else
        {
            EnemyUnion.AddCircle(transform, sancRadius);
        }
    }

    public void RemoveUnion(Transform transform, bool isTagAlly)
    {
        if(isTagAlly)
        {
            AllyUnion.RemoveCircle(transform);
        }
        else
        {
            EnemyUnion.RemoveCircle(transform);
        }
    }

    public void AllyCastleTierUp()
    {
        Castle allyCastle = AllyCastle.GetComponent<Castle>();
        allyCastle.RequestTierUp();
    }
}
