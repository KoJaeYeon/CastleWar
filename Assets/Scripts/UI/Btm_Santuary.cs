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
    Collider[] hitColliders = new Collider[2]; // 충돌을 저장할 배열

    int tempLayer;
    int _index = 7;
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
        _spawnedUnit = SpawnManager.Instance.OnCalled_GetUnit(7);

        _spawnedUnit.layer = LayerMask.NameToLayer("Default");

    }

    public void OnPointerUp()
    {
        isDown = false;
        if (_spawnedUnit == null) return;

        

        //청사진이 적용되어 있을때
        if (_spawnedUnit.activeSelf)
        {
            //조건 충족시 유닛 소환
            if (GameManager.Instance.RequestManaCheck(75))
            {
                GameManager.Instance.RequestManaUse(-75);
                //_spawnedUnit.layer = tempLayer;
                //var unit = _spawnedUnit.GetComponent<Unit>();
                //unit?.StartState();
                //_spawnedUnit = null;

                {
                    Vector3 touchPos = _spawnedUnit.transform.position;

                    ReturnSanc();
                    TcpSender.Instance.RequestSpawnUnit(touchPos, _index);
                }
            }
            else
            {
                if (_spawnedUnit != null)
                {
                    ReturnSanc();
                }
                return;
            }
        }

    }

    public void ExecuteUpdate(Vector3 touchPos)
    {
        if (isDown)
        {
            Vector3 roundedVector = new Vector3(2 * Mathf.Round(touchPos.x / 2), 0, 2 * Mathf.Round(touchPos.z / 2));

            string targetLayer = "Resource";
            int layerMask = LayerMask.GetMask(targetLayer);
            int hitCount = Physics.OverlapSphereNonAlloc(roundedVector, 2f, hitColliders, layerMask);
            if (hitCount > 0)
            {
                //설치된 마나성소가 있는지 추가 검사
                string[] targetLayers = new[] { "EnemyBuilding", "AllyBuilding"};
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
                    ReturnSanc();
                }
            }
        }
    }

    public void OnPointerExit()
    {
        if (_spawnedUnit != null)
        {
            isDown = false;

            ReturnSanc();
        }
    }

    private void ReturnSanc()
    {
        //안쓰는 유닛 반환
        SpawnManager.Instance.OnCalled_ReturnUnit(7, _spawnedUnit);
        _spawnedUnit.SetActive(false);
        _spawnedUnit = null;
    }
}
