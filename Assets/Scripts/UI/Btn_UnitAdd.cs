using System.Collections;
using UnityEngine;

public enum TouchType
{
    Unit, NotUnit, Mana
}
public class Btn_UnitAdd : MonoBehaviour, ISelectable
{
    TouchType touchType = TouchType.Unit;
    bool isDown = false;
    int _index;

    private float _spawnInterval = 0.1f; // 소환 주기 (0.1초)
    private float _lastSpawnTime = 0f;

    GameObject _spawnedUnit;
    Collider[] hitColliders = new Collider[2]; // 충돌을 저장할 배열
    Coroutine coroutine;

    //배치 시 적용되는 레이어
    int originLayer; // 배치 후 적용
    int defaultLayer; // 배치 전 적용 충돌무시

    //소환 시 소모되는 값
    int cost;
    int population;

    public void SetInit(int index, int id)
    {
        _index = index;
        var unitData = DatabaseManager.Instance.OnGetUnitData(id);
        cost = unitData.cost;
        population = unitData.Population;
        switch (unitData.unitType)
        {
            case UnitType.Ground:
                originLayer =  LayerMask.NameToLayer("AllyGroundUnit");
                touchType = TouchType.Unit;
                break;
            case UnitType.Air:
                originLayer = LayerMask.NameToLayer("AllyAirUnit");
                touchType = TouchType.Unit;
                break;
            default:
                originLayer = LayerMask.NameToLayer("AllyBuilding");
                touchType = TouchType.NotUnit;
                break;
        }
        defaultLayer = LayerMask.NameToLayer("DeadUnit");
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

    void MoveImg(bool isImgUp)
    {
        Transform targetTrans = transform.GetChild(0);
        if (coroutine != null) StopCoroutine(coroutine);
        Vector3 target = isImgUp ? Vector3.up * 20 : Vector3.zero;
        coroutine = StartCoroutine(targetGraphic(targetTrans, target));
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

    public void OnPointerDown()
    {
        isDown = true;
        _spawnedUnit = SpawnManager.Instance.OnCalled_GetUnit(_index);

        _spawnedUnit.layer = defaultLayer;
        if (touchType == TouchType.Unit)
        {
            _spawnedUnit.SetActive(true);
        }
    }

    public void OnPointerUp()
    {
        isDown = false;
        if (_spawnedUnit == null) return;

        if (touchType == TouchType.Unit)
        {
            ReturnUnit();
        }
        else
        {
            //청사진 없으면 소환 불가
            if(_spawnedUnit.activeSelf)
            {
                if (GameManager.Instance.RequestManaCheck(cost) && GameManager.Instance.RequestPopulationCheck(population))
                {
                    GameManager.Instance.RequestPopulationUse(population);
                    GameManager.Instance.RequestManaUse(cost * -1);
                    _spawnedUnit.layer = originLayer;
                    var unit = _spawnedUnit.GetComponent<Unit>();
                    unit?.StartState();
                    _spawnedUnit = null;
                }
                else
                {
                    ReturnUnit();
                }
            }
        }
    }

    public void ExecuteUpdate(Vector3 touchPos)
    {
        if (isDown)
        {
            // 유닛 드래그
            if (_spawnedUnit == null)
            {
                return;
            }

            if (touchType == TouchType.Unit)
            {
                // 유닛 드래그
                string[] targetLayers = new[] { "EnemyBuilding", "AllyBuilding", "Border", "Resource" };
                int layerMask = LayerMask.GetMask(targetLayers);

                touchPos.y = 0;
                int hitCount = Physics.OverlapSphereNonAlloc(touchPos, 0.1f, hitColliders, layerMask);

                if (hitCount == 0)
                {
                    _spawnedUnit.transform.position = touchPos;
                }

                if (Time.time - _lastSpawnTime<_spawnInterval) return; // 소환 주기가 되지 않으면 반환
                _lastSpawnTime = Time.time;

                if (GameManager.Instance.RequestManaCheck(cost) && GameManager.Instance.RequestPopulationCheck(population))
                {
                    GameManager.Instance.RequestPopulationUse(population);
                    GameManager.Instance.RequestManaUse(cost * -1);
                    var unit = _spawnedUnit.GetComponent<Unit>();
                    unit?.StartState();
                    _spawnedUnit.layer = originLayer;

                    //대기 유닛 치환
                    _spawnedUnit = SpawnManager.Instance.OnCalled_GetUnit(_index);
                    _spawnedUnit.layer = defaultLayer;
                    _spawnedUnit.SetActive(true);
                    touchPos.y = 0;
                    _spawnedUnit.transform.position = touchPos;
                }
            }
            else if (touchType == TouchType.NotUnit)
            {
                Vector3 roundedVector = new Vector3(2 * Mathf.Round(touchPos.x / 2), 0, 2 * Mathf.Round(touchPos.z / 2));

                string[] targetLayers = new[] { "EnemyBuilding", "AllyBuilding", "Border","Bridge", "Resource" };
                int layerMask = LayerMask.GetMask(targetLayers);

                int hitCount = Physics.OverlapSphereNonAlloc(roundedVector, 3f, hitColliders, layerMask);

                if (hitCount == 0)
                {
                    _spawnedUnit.SetActive(true);
                    _spawnedUnit.transform.position = roundedVector;
                }
            }
        }
    }

    public void OnPointerExit()
    {
        if (_spawnedUnit != null)
        {
            isDown = false;

            ReturnUnit();
        }
    }

    void ReturnUnit()
    {
        if(_spawnedUnit != null)
        {
            //안쓰는 유닛 반환
            SpawnManager.Instance.OnCalled_ReturnUnit(_index, _spawnedUnit);
            _spawnedUnit.SetActive(false);
            _spawnedUnit = null;
        }
    }
}
