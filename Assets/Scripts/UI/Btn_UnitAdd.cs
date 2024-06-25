using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TouchType
{
    Unit, NotUnit, Mana
}
public class Btn_UnitAdd : MonoBehaviour,ISelectable
{
    TouchType touchType = TouchType.Unit;
    bool isDown = false;
    int _index;
    public void SetInit(int index, int id)
    {
        _index = index;
        var unitData = DatabaseManager.Instance.OnGetUnitData(id);
        switch(unitData.unitType)
        {
            case UnitType.Ground:
            case UnitType.Air:
                touchType = TouchType.Unit;
                break;
            default:
                touchType = TouchType.NotUnit;
                break;
        }
    }
    public void Canceled()
    {
        Debug.Log($"{name} Canceled");
    }

    public void Selected()
    {
        Debug.Log($"{name} Selected");
    }

    public void OnPointerDown()
    {
        isDown = true;
    }

    public void OnPointerUp()
    {
        isDown = false;
    }

    public void ExecuteUpdate(Vector3 touchPos)
    {
        if(isDown )
        {
            if(touchType == TouchType.Unit)
            {
                var spawnUnit = SpawnManager.Instance.OnCalled_GetUnit(_index);
                spawnUnit.SetActive(true);
                touchPos.y = 0;
                spawnUnit.transform.position = touchPos;
                Debug.Log($"Spawn : {touchPos}");
            }
        }
    }
}
