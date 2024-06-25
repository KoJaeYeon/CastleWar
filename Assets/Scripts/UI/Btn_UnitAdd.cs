using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Btn_UnitAdd : MonoBehaviour,ISelectable
{
    public void Canceled()
    {
        Debug.Log($"{name} Canceled");
    }

    public void Selected()
    {
        Debug.Log($"{name} Selected");
    }

    void ISelectable.Canceled()
    {
        throw new System.NotImplementedException();
    }

    void ISelectable.OnPointerDown()
    {
        throw new System.NotImplementedException();
    }

    void ISelectable.OnPointerUp()
    {
        throw new System.NotImplementedException();
    }

    void ISelectable.Selected()
    {
        throw new System.NotImplementedException();
    }
}
