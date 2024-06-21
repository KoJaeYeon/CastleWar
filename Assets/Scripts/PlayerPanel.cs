using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPanel : MonoBehaviour
{
    public void OnClick_Btn_Retreat()
    {
        UnitManager.Instance.OnCalled_Retreat(isAlly:true);
    }
}
