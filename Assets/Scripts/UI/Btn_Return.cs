using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Btn_Return : MonoBehaviour
{
    Button button;
    [SerializeField] GameObject RetreatBtn;
    [SerializeField] GameObject CancelBtn;
    [SerializeField] GameObject ReturnTimer;
    [SerializeField] Image maskImg;
    [SerializeField] TextMeshProUGUI ReturnTimeText;

    readonly float retreatTime = 20f;
    float elapsedTime;
    bool isUpdate = false;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick_Btn_Retreat);
        UnitManager.Instance.ChangeCancelButtonCallback(OnClick_Btn_Cancel);
    }

    private void Update()
    {
        if (!isUpdate) return;
        // 마스크 이미지 변경
        elapsedTime += Time.deltaTime;
        maskImg.fillAmount = (retreatTime - elapsedTime) / retreatTime;

        //타이머 Text 변경
        float leftTime = Mathf.Ceil(retreatTime - elapsedTime);
        ReturnTimeText.text = leftTime.ToString();

        if (elapsedTime > retreatTime)
        {
            ReturnTimer.SetActive(false);

            isUpdate = false;
            button.interactable = true;
        }
    }

    public void OnClick_Btn_Retreat()
    {
        //귀환할 유닛이 없으면 행동취소
        if (UnitManager.Instance.CheckCanRetreat()) return;

        //버튼 메서드 변경
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnClick_Btn_Cancel);

        //후퇴명령
        //UnitManager.Instance.OnCalled_Retreat(isTagAlly: true);
        TcpSender.Instance.RequestCommand(1);

        //이미지 변경
        RetreatBtn.SetActive(false);
        CancelBtn.SetActive(true);

        ReturnTimer.SetActive(true);

        //마스크 이미지
        isUpdate = true;
        elapsedTime = 0;
    }
    public void OnClick_Btn_Cancel()
    {
        //버튼 메서드 변경
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnClick_Btn_Retreat);

        //취소명령
        //UnitManager.Instance.OnCalled_Cancel(isTagAlly: true);
        TcpSender.Instance.RequestCommand(2);

        //이미지 변경
        RetreatBtn.SetActive(true);
        CancelBtn.SetActive(false);

        if(isUpdate) button.interactable = false;
    }
}
