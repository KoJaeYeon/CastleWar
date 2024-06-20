using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    private static UnitAttackManager _instance;

    public static UnitAttackManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameObject("UnitAttackManager").AddComponent<UnitAttackManager>();
            }
            return _instance;
        }
    }

    Dictionary<int, Unit> _frinedUnitDic = new Dictionary<int, Unit>();

    public void AddFriendUnit(Unit unit)
    {
        unit.GetInstanceID();
    }
}
