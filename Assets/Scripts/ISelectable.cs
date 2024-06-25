using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISelectable
{
    public void Selected();
    public void Canceled();

    public void OnPointerDown();
    public void OnPointerUp();

    public void ExecuteUpdate(Vector3 vector3);
}
