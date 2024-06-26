using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TouchType
{
    Unit, NotUnit, Mana
}
public class Btn_UnitAdd : MonoBehaviour,ISelectable
{
    TouchType touchType = TouchType.Unit;
    bool isDown = false;
    int _index;

    private float _spawnInterval = 0.1f; // 소환 주기 (0.1초)
    private float _lastSpawnTime = 0f;

    GameObject _spawnedUnit;

    public void SetInit(int index, int id)
    {
        _index = index;
        var unitData = DatabaseManager.Instance.OnGetUnitData(id);
        switch(unitData.unitType)
        {
            case UnitType.Ground:
            case UnitType.Air:
                touchType = TouchType.Unit;
                break;
            default:
                touchType = TouchType.NotUnit;
                break;
        }
    }
    public void Canceled()
    {
        Debug.Log($"{name} Canceled");
    }

    public void Selected()
    {
        Debug.Log($"{name} Selected");
    }

    public void OnPointerDown()
    {
        isDown = true;
        _spawnedUnit = SpawnManager.Instance.OnCalled_GetUnit(_index);
        _spawnedUnit.SetActive(true);

    }

    public void OnPointerUp()
    {
        isDown = false;
        if(touchType == TouchType.Unit)
        {
            //안쓰는 유닛 반환
            SpawnManager.Instance.OnCalled_ReturnUnit(_index, _spawnedUnit);
            _spawnedUnit.SetActive(false);
        }
    }

    public void ExecuteUpdate(Vector3 touchPos)
    {
        if(isDown )
        {
            // 유닛 드래그
            if(_spawnedUnit != null)
            {
                touchPos.y = 0;
                _spawnedUnit.transform.position = touchPos;
            }

            if (touchType == TouchType.Unit)
            {
                if (Time.time - _lastSpawnTime<_spawnInterval) return; // 소환 주기가 되지 않으면 반환
                _lastSpawnTime = Time.time;

                Debug.Log("entered");
                //[TODO]
                //조건 충족시 유닛 소환



                //대기 유닛 치환
                _spawnedUnit = SpawnManager.Instance.OnCalled_GetUnit(_index);            
                _spawnedUnit.SetActive(true);
                touchPos.y = 0;
                _spawnedUnit.transform.position = touchPos;

                Debug.Log($"Spawn : {touchPos}");
            }
        }
    }
}
