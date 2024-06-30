using System;
using System.Collections;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Btm_Sanctuary : MonoBehaviour, ISelectable
{
    bool isDown = false;

    GameObject _spawnedUnit;
    Collider[] hitColliders = new Collider[2]; // �浹�� ������ �迭

    int tempLayer;
    Coroutine coroutine;

    private void Awake()
    {
        Button button = GetComponent<Button>();
        if (button != null)
        {
            Action spawnAction = () =>
            {
                TouchManager.Instance.ChangeSelectalble(this);
            };
            button.onClick.AddListener(new UnityAction(spawnAction));
        }
        tempLayer = LayerMask.NameToLayer("AllyBuilding");
    }

    public void Canceled()
    {
        Debug.Log($"{name} Canceled");
        MoveImg(isImgUp: false);
    }

    public void Selected()
    {
        Debug.Log($"{name} Selected");
        MoveImg(isImgUp: true);
    }

    IEnumerator targetGraphic(Transform targetTrans, Vector3 targetPos)
    {
        while (true)
        {
            targetTrans.localPosition = Vector3.Lerp(targetTrans.localPosition, targetPos, 0.1f);
            yield return null;
            if (Vector3.Distance(targetTrans.localPosition, targetPos) < 0.1f)
            {
                break;
            }
        }
    }

    void MoveImg(bool isImgUp)
    {
        Transform targetTrans = transform.GetChild(0);
        if (coroutine != null) StopCoroutine(coroutine);
        Vector3 target = isImgUp ? Vector3.up * 20 : Vector3.zero;
        coroutine = StartCoroutine(targetGraphic(targetTrans, target));
    }

    public void OnPointerDown()
    {
        isDown = true;
        _spawnedUnit = SpawnManager.Instance.OnCalled_GetSanctuary();

        _spawnedUnit.layer = LayerMask.NameToLayer("Default");

    }

    public void OnPointerUp()
    {
        isDown = false;
        if (_spawnedUnit == null) return;

        _spawnedUnit.layer = tempLayer;

        //[TODO]���� ������ ���� ��ȯ
        if (_spawnedUnit.activeSelf)
        {
            var unit = _spawnedUnit.GetComponent<Unit>();
            unit?.StartState();
            _spawnedUnit = null;
        }

    }

    public void ExecuteUpdate(Vector3 touchPos)
    {
        if (isDown)
        {
            Vector3 roundedVector = new Vector3(2 * Mathf.Round(touchPos.x / 2), 0, 2 * Mathf.Round(touchPos.z / 2));

            string targetLayer = "Resource";
            int layerMask = LayerMask.GetMask(targetLayer);
            int hitCount = Physics.OverlapSphereNonAlloc(roundedVector, 3f, hitColliders, layerMask);
            Debug.Log(hitCount);
            if (hitCount > 0)
            {
                //��ġ�� �������Ұ� �ִ��� �߰� �˻�
                string[] targetLayers = new[] { "EnemyBuilding", "AllyBuilding", "Border" };
                layerMask = LayerMask.GetMask(targetLayers);
                Vector3 sanctuaryPos = hitColliders[0].transform.position;
                hitCount = Physics.OverlapSphereNonAlloc(sanctuaryPos, 0.2f, hitColliders, layerMask);

                if (hitCount == 0)
                {
                    if(_spawnedUnit == null)
                    {
                        OnPointerDown();
                    }

                    _spawnedUnit.SetActive(true);
                    _spawnedUnit.transform.position = hitColliders[0].transform.position;
                }

            }
            else
            {
                if (_spawnedUnit != null)
                {
                    //�Ⱦ��� ���� ��ȯ
                    SpawnManager.Instance.OnCalled_ReturnSanc(_spawnedUnit);
                    _spawnedUnit.SetActive(false);
                    _spawnedUnit = null;
                }
            }
        }
    }

    public void OnPointerExit()
    {
        if (_spawnedUnit != null)
        {
            isDown = false;

            //�Ⱦ��� ���� ��ȯ
            SpawnManager.Instance.OnCalled_ReturnSanc(_spawnedUnit);
            _spawnedUnit.SetActive(false);
            _spawnedUnit = null;
        }
    }
}
