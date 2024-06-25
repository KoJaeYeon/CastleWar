using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchManager : MonoBehaviour
{
    private static TouchManager _instance;

    public static TouchManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameObject("TouchManager").AddComponent<TouchManager>();
            }
            return _instance;
        }

    }
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
