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
    public void SetInit()
    {

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
            Debug.Log($"Spawn : {touchPos}");
        }
    }
}
