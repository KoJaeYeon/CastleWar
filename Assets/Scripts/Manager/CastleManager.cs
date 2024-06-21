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
        if(_instance == null)
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
}
