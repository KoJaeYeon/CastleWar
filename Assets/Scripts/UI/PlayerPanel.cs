using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPanel : MonoBehaviour
{
    [SerializeField] AddPanel AddPanel;
    [SerializeField] Transform Content;
    public void OnClick_Btn_Retreat()
    {
        UnitManager.Instance.OnCalled_Retreat(isAlly:true);
    }

    public void OnClick_Btn_Cancel()
    {
        UnitManager.Instance.OnCalled_Cancel(isAlly: true);
    }

    public void OnClick_AddPanel_Active(int index)
    {
        AddPanel.OnClick_ActivePanel(index, Content.GetChild(index));
    }
}
