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
            Content.GetChild(i).GetChild(0).gameObject.SetActive(false); //�̹��� ��Ȱ��ȭ
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
        StartCoroutine(CardAnimation(slotInitPos, targetObjTrans, id)); //ī�� ��� �ڷ�ƾ ����
        if(index != 5) // ���������� ����
        {
            EnableNextSlot(index + 1); // �������� ��� Ȱ��ȭ
        }
        else
        {
            AddPanel.OnCalled_SetActive(false);
        }
        
        
    }

    void EnableNextSlot(int index)
    {
        Content.GetChild(index).GetChild(0).gameObject.SetActive(true); //�̹��� Ȱ��ȭ
        Content.GetChild(index).GetComponent<Button>().interactable = true;
    }
    IEnumerator CardAnimation(Vector3 InitPos, Transform targetTrans, int id)
    {
        Vector3 targetPos = targetTrans.position;

        float duration = 1.0f; // �̵� �ð� 1��
        float elapsedTime = 0.0f; // ��� �ð�

        // ��� ���
        float a = (targetPos.y - InitPos.y) / 0.4f;
        float b = InitPos.y - 0.09f * a;

        GameObject image = Instantiate(CardPrefab, transform); // ī�� ������ �ν��Ͻ�ȭ
        image.transform.position = InitPos; // �ʱ� ��ġ ����
        image.GetComponent<Image>().sprite = DatabaseManager.Instance.GetSpriteData(id); // ��������Ʈ �����ϱ�

        while (elapsedTime <= duration) // ī�� ��� �ִϸ��̼�(�������)
        {
            elapsedTime += Time.deltaTime; // ��� �ð� ������Ʈ
            float t = elapsedTime / duration; // ����ȭ�� �ð� t ���

            float x = Mathf.Lerp(InitPos.x, targetPos.x, t); // x ��ġ ���� ����
            float y = a * Mathf.Pow(t - 0.3f, 2) + b; // y ��ġ ������ ���
            image.transform.position = new Vector3(x, y, 0); // �̹��� ��ġ ������Ʈ

            if (t >= 1.0f)
            {
                Debug.Log(image.transform.position);
                break; // t�� 1�� �����ϸ� ���� ����
            }
            yield return null; // ���� �����ӱ��� ���
        }


        while (elapsedTime <= duration) // ī�� ��� ���ð�
        {
            elapsedTime += Time.deltaTime; // ��� �ð� ������Ʈ


            if (elapsedTime <= 10f)
            {
                Debug.Log(image.transform.position);
            }
            else
            {
                break; // elapsedTime�� 10�� �����ϸ� ���� ����
            }
            yield return null; // ���� �����ӱ��� ���
        }
    }


}