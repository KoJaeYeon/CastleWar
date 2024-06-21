using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPanel : MonoBehaviour
{
    public void OnClick_Btn_Retreat()
    {
        UnitManager.Instance.OnCalled_Retreat(isAlly:true);
    }

    public void OnClick_Btn_Cancel()
    {
        UnitManager.Instance.OnCalled_Cancel(isAlly: true);
    }
}
