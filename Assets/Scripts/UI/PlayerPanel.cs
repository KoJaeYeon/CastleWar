using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPanel : MonoBehaviour
{
    [SerializeField] AddPanel AddPanel;
    [SerializeField] Transform Content;
    [SerializeField] GameObject CardPrefab;

    private void Awake()
    {
        Content.GetChild(0).GetChild(0).gameObject.SetActive(true);
        for (int i = 1; i< 6; i++)
        {
            Content.GetChild(i).GetChild(0).gameObject.SetActive(false); //이미지 비활성화
            Content.GetChild(i).GetComponent<Button>().interactable = false;
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

    public void OnClick_AddPanel_Active(int index)
    {
        AddPanel.OnClick_ActivePanel();
    }

    public void OnCalled_Added(int index, Vector3 slotInitPos, int id)
    {
        Debug.Log(slotInitPos);
        Transform targetObjTrans = Content.GetChild(index);
        StartCoroutine(CardAnimation(slotInitPos, targetObjTrans, id)); //카드 등록 코루틴 실행
        if(index != 5) // 마지막슬롯 제외
        {
            EnableNextSlot(index + 1); // 다음슬롯 대기 활성화
        }
        else
        {
            AddPanel.OnCalled_SetActive(false);
        }
        
        
    }

    void EnableNextSlot(int index)
    {
        Content.GetChild(index).GetChild(0).gameObject.SetActive(true); //이미지 활성화
        Content.GetChild(index).GetComponent<Button>().interactable = true;
    }
    IEnumerator CardAnimation(Vector3 InitPos, Transform targetTrans, int id)
    {
        Vector3 targetPos = targetTrans.position;

        float duration = 1.0f; // 이동 시간 1초
        float elapsedTime = 0.0f; // 경과 시간

        // 계수 계산
        float a = (targetPos.y - InitPos.y) / 0.4f;
        float b = InitPos.y - 0.09f * a;

        GameObject image = Instantiate(CardPrefab, transform); // 카드 프리팹 인스턴스화
        image.transform.position = InitPos; // 초기 위치 설정
        image.GetComponent<Image>().sprite = DatabaseManager.Instance.GetSpriteData(id); // 스프라이트 적용하기

        while (elapsedTime <= duration) // 카드 등록 애니메이션(날라오기)
        {
            elapsedTime += Time.deltaTime; // 경과 시간 업데이트
            float t = elapsedTime / duration; // 정규화된 시간 t 계산

            float x = Mathf.Lerp(InitPos.x, targetPos.x, t); // x 위치 선형 보간
            float y = a * Mathf.Pow(t - 0.3f, 2) + b; // y 위치 포물선 계산
            image.transform.position = new Vector3(x, y, 0); // 이미지 위치 업데이트

            if (t >= 1.0f)
            {
                Debug.Log(image.transform.position);
                break; // t가 1에 도달하면 루프 종료
            }
            yield return null; // 다음 프레임까지 대기
        }


        while (elapsedTime <= duration) // 카드 등록 대기시간
        {
            elapsedTime += Time.deltaTime; // 경과 시간 업데이트


            if (elapsedTime <= 10f)
            {
                Debug.Log(image.transform.position);
            }
            else
            {
                break; // elapsedTime가 10에 도달하면 루프 종료
            }
            yield return null; // 다음 프레임까지 대기
        }
    }


}