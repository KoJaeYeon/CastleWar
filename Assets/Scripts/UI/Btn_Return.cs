using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Btn_Return : MonoBehaviour
{
    Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick_Btn_Retreat);
        UnitManager.Instance.ChangeCancelButtonCallback(OnClick_Btn_Cancel);
    }

    public void OnClick_Btn_Retreat()
    {
        //버튼 메서드 변경
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnClick_Btn_Cancel);

        //후퇴명령
        UnitManager.Instance.OnCalled_Retreat(isAlly: true);
    }
    public void OnClick_Btn_Cancel()
    {
        //버튼 메서드 변경
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnClick_Btn_Retreat);

        //취소명령
        UnitManager.Instance.OnCalled_Cancel(isAlly: true);
    }

}
