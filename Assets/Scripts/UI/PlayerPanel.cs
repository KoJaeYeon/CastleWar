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
                plusImage.SetActive(false); //�̹��� ��Ȱ��ȭ
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
        Debug.Log(slotInitPos);
        Transform targetObjTrans = Content.GetChild(index);
        StartCoroutine(CardAnimation(slotInitPos, targetObjTrans, id, index)); //ī�� ��� �ڷ�ƾ ����
        SetSlotButtonIneractable(index, false);
        if(index != 5) // ���������� ����
        {
            EnableNextSlot(index + 1); // �������� ��� Ȱ��ȭ
        }
        else
        {
            AddPanel.OnClick_DeactivePanel();
        }
        
        
    }

    void EnableNextSlot(int index)
    {
        var nextButtonSlot = Content.GetChild(index).GetChild(0).gameObject;
        nextButtonSlot.SetActive(true); //�̹��� Ȱ��ȭ
        SetSlotButtonIneractable(index, true);
    }

    void SetSlotButtonIneractable(int index, bool ineractable)
    {
        Button targetBtn = Content.GetChild(index).GetComponent<Button>();
        targetBtn.interactable = ineractable; //��ư Ȱ��ȭ
    }


    IEnumerator CardAnimation(Vector3 InitPos, Transform targetTrans, int id, int index)
    {
        Vector3 targetPos = targetTrans.position;
        targetTrans.localScale = Vector3.one * 1.2f;

        float duration = 1.0f; // �̵� �ð� 1��
        float elapsedTime = 0.0f; // ��� �ð�

        // ��� ���
        float a = (targetPos.y - InitPos.y) / 0.4f;
        float b = InitPos.y - 0.09f * a;

        GameObject cardPrefab = Instantiate(CardPrefab, transform); // ī�� ������ �ν��Ͻ�ȭ
        cardPrefab.transform.position = InitPos; // �ʱ� ��ġ ����

        Image image = cardPrefab.GetComponent<Image>();
        image.sprite = DatabaseManager.Instance.GetSpriteData(id); // ��������Ʈ �����ϱ�

        while (elapsedTime <= duration) // ī�� ��� �ִϸ��̼�(�������)
        {
            elapsedTime += Time.deltaTime; // ��� �ð� ������Ʈ
            float t = elapsedTime / duration; // ����ȭ�� �ð� t ���

            float x = Mathf.Lerp(InitPos.x, targetPos.x, t); // x ��ġ ���� ����
            float y = a * Mathf.Pow(t - 0.3f, 2) + b; // y ��ġ ������ ���
            cardPrefab.transform.position = new Vector3(x, y, 0); // �̹��� ��ġ ������Ʈ

            if (t >= 1.0f)
            {
                ActiveButtonImageWithMask(targetTrans, id);
                Destroy(cardPrefab);
                break; // t�� 1�� �����ϸ� ���� ����
            }
            yield return null; // ���� �����ӱ��� ���
        }

        TextMeshProUGUI maskText = targetTrans.GetComponentInChildren<TextMeshProUGUI>();

        while (elapsedTime <= 10f) // ī�� ��� ���ð�
        {
            elapsedTime += Time.deltaTime; // ��� �ð� ������Ʈ


            if (elapsedTime <= 10f)
            {
                SetMaskTextLeftTime(maskText,elapsedTime);
            }
            else
            {
                ActiveButtonOnCoolDown(targetTrans,index);
                break; // elapsedTime�� 10�ʿ� �����ϸ� ���� ����
            }
            yield return null; // ���� �����ӱ��� ���
        }
    }

    #region Button
    void ActiveButtonImageWithMask(Transform targetTrans, int id)
    {
        Image buttonImage = targetTrans.GetComponent<Image>();
        buttonImage.sprite = DatabaseManager.Instance.GetSpriteData(id);

        var plusImage = targetTrans.GetChild(0).gameObject;
        var maskImage = targetTrans.GetChild(1).gameObject;

        plusImage.SetActive(false); // �÷��� �̹��� ��Ȱ��ȭ
        maskImage.SetActive(true); // ����ũ �̹��� Ȱ��ȭ

        targetTrans.localScale = Vector3.one;
    }

    void ActiveButtonOnCoolDown(Transform targetTrans, int index)
    {
        Button spawnButton = targetTrans.GetComponent<Button>();
        if (spawnButton != null)
        {
            spawnButton.interactable = true;
            spawnButton.onClick.RemoveAllListeners(); // ���� ������ ��� ����

            Action spawnAction = () =>
            {
                //[TODO] Ŭ���ϸ� ��ȯ�����·� �����ؾ���
                SpawnManager.Instance.OnCalled_GetUnit(index).SetActive(true);
            };
            spawnButton.onClick.AddListener(new UnityAction(spawnAction)); // �� �׼� ������ �߰�

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