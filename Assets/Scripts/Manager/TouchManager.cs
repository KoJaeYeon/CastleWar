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
        if (_instance == null || _instance == this)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private ISelectable _selectable;

    public void ChangeSelectalble(ISelectable selectable)
    {
        Debug.Log(selectable);
        if(_selectable != null)
        {
            _selectable.Canceled();
        }        
        _selectable = selectable;
        if(_selectable != null)
        {
            _selectable.Selected();
        }
        
    }
}
