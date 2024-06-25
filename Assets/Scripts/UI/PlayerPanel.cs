using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayerPanel : MonoBehaviour
{
    [SerializeField] AddPanel AddPanel;
    [SerializeField] Transform Content;
    [SerializeField] GameObject CardPrefab;

    private void Awake()
    {
        for (int i = 0; i< 6; i++)
        {
            Button button = Content.GetChild(i).GetComponent<Button>();

            if (i != 0)
            {
                var plusImage = Content.GetChild(i).GetChild(0).gameObject;
                plusImage.SetActive(false); //이미지 비활성화
                button.interactable = false;
            }

            button.onClick.AddListener(OnClick_AddPanel_Active);
        }
    }
    public void OnClick_Btn_Retreat()
    {
        UnitManager.Instance.OnCalled_Retreat(isAlly: true);
    }

    public void OnClick_Btn_Cancel()
    {
        UnitManager.Instance.OnCalled_Cancel(isAlly: true);
    }

    public void OnClick_AddPanel_Active()
    {
        AddPanel.OnClick_ActivePanel();
    }

    public void OnCalled_Added(int index, Vector3 slotInitPos, int id)
    {
        Transform targetObjTrans = Content.GetChild(index);
        StartCoroutine(CardAnimation(slotInitPos, targetObjTrans, id, index)); //카드 등록 코루틴 실행
        SetSlotButtonIneractable(index, false);
        if(index != 5) // 마지막슬롯 제외
        {
            EnableNextSlot(index + 1); // 다음슬롯 대기 활성화
        }
        else
        {
            AddPanel.OnClick_DeactivePanel();
        }
        
        
    }

    void EnableNextSlot(int index)
    {
        var nextButtonSlot = Content.GetChild(index).GetChild(0).gameObject;
        nextButtonSlot.SetActive(true); //이미지 활성화
        SetSlotButtonIneractable(index, true);
    }

    void SetSlotButtonIneractable(int index, bool ineractable)
    {
        Button targetBtn = Content.GetChild(index).GetComponent<Button>();
        targetBtn.interactable = ineractable; //버튼 활성화
    }


    IEnumerator CardAnimation(Vector3 InitPos, Transform targetTrans, int id, int index)
    {
        Vector3 targetPos = targetTrans.position;
        targetTrans.localScale = Vector3.one * 1.2f;

        float duration = 1.0f; // 이동 시간 1초
        float elapsedTime = 0.0f; // 경과 시간

        // 계수 계산
        float a = (targetPos.y - InitPos.y) / 0.4f;
        float b = InitPos.y - 0.09f * a;

        GameObject cardPrefab = Instantiate(CardPrefab, transform); // 카드 프리팹 인스턴스화
        cardPrefab.transform.position = InitPos; // 초기 위치 설정

        Image image = cardPrefab.GetComponent<Image>();
        image.sprite = DatabaseManager.Instance.GetSpriteData(id); // 스프라이트 적용하기

        while (elapsedTime <= duration) // 카드 등록 애니메이션(날라오기)
        {
            elapsedTime += Time.deltaTime; // 경과 시간 업데이트
            float t = elapsedTime / duration; // 정규화된 시간 t 계산

            float x = Mathf.Lerp(InitPos.x, targetPos.x, t); // x 위치 선형 보간
            float y = a * Mathf.Pow(t - 0.3f, 2) + b; // y 위치 포물선 계산
            cardPrefab.transform.position = new Vector3(x, y, 0); // 이미지 위치 업데이트

            if (t >= 1.0f)
            {
                ActiveButtonImageWithMask(targetTrans, id);
                Destroy(cardPrefab);
                break; // t가 1에 도달하면 루프 종료
            }
            yield return null; // 다음 프레임까지 대기
        }

        TextMeshProUGUI maskText = targetTrans.GetComponentInChildren<TextMeshProUGUI>();

        while (elapsedTime <= 10f) // 카드 등록 대기시간
        {
            elapsedTime += Time.deltaTime; // 경과 시간 업데이트


            if (elapsedTime <= 10f)
            {
                SetMaskTextLeftTime(maskText,elapsedTime);
            }
            else
            {
                ActiveButtonOnCoolDown(targetTrans,index);
                break; // elapsedTime이 10초에 도달하면 루프 종료
            }
            yield return null; // 다음 프레임까지 대기
        }
    }

    #region Button
    void ActiveButtonImageWithMask(Transform targetTrans, int id)
    {
        Image buttonImage = targetTrans.GetComponent<Image>();
        buttonImage.sprite = DatabaseManager.Instance.GetSpriteData(id);

        var plusImage = targetTrans.GetChild(0).gameObject;
        var maskImage = targetTrans.GetChild(1).gameObject;

        plusImage.SetActive(false); // 플러스 이미지 비활성화
        maskImage.SetActive(true); // 마스크 이미지 활성화

        targetTrans.localScale = Vector3.one;
    }

    void ActiveButtonOnCoolDown(Transform targetTrans, int index)
    {
        Button spawnButton = targetTrans.GetComponent<Button>();
        if (spawnButton != null)
        {
            spawnButton.interactable = true;
            spawnButton.onClick.RemoveAllListeners(); // 기존 리스너 모두 제거

            Action spawnAction = () =>
            {
                ISelectable selectable = spawnButton.transform.GetComponent<ISelectable>();
                TouchManager.Instance.ChangeSelectalble(selectable);
            };
            spawnButton.onClick.AddListener(new UnityAction(spawnAction)); // 새 액션 리스너 추가

            var maskImage = targetTrans.GetChild(1).gameObject;
            maskImage.SetActive(false);

        }
    }
    void SetMaskTextLeftTime(TextMeshProUGUI maskText, float elapsedTime)
    {
        if(maskText != null)
        {
            maskText.text =  (10-elapsedTime).ToString("0");
        }
        else
        {
            Debug.LogError("MaskText is Null");
        }
        
    }

    #endregion

}