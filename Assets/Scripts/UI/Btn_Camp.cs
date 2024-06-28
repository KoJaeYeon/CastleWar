﻿using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Btn_Camp : MonoBehaviour, ISelectable
{
    bool isDown = false;

    GameObject _spawnedUnit;
    Collider[] hitColliders = new Collider[2]; // 충돌을 저장할 배열

    int tempLayer;

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
    }

    public void Selected()
    {
        Debug.Log($"{name} Selected");
    }

    public void OnPointerDown()
    {
        isDown = true;
        _spawnedUnit = SpawnManager.Instance.OnCalled_GetCamp();

        _spawnedUnit.layer = LayerMask.NameToLayer("Default");

    }

    public void OnPointerUp()
    {
        isDown = false;
        if (_spawnedUnit == null) return;

        _spawnedUnit.layer = tempLayer;

        //[TODO]조건 충족시 유닛 소환
        if(_spawnedUnit.activeSelf)
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
            // 유닛 드래그
            if (_spawnedUnit == null)
            {
                return;
            }
            Vector3 roundedVector = new Vector3(2 * Mathf.Round(touchPos.x / 2), 0, 2 * Mathf.Round(touchPos.z / 2));

            string[] targetLayers = new[] { "EnemyBuilding", "AllyBuilding", "Border" };
            int layerMask = LayerMask.GetMask(targetLayers);

            int hitCount = Physics.OverlapSphereNonAlloc(roundedVector, 3f, hitColliders, layerMask);

            if (hitCount == 0)
            {
                _spawnedUnit.SetActive(true);
                _spawnedUnit.transform.position = roundedVector;
            }
        }
    }

    public void OnPointerExit()
    {
        if (_spawnedUnit != null)
        {
            isDown = false;

            //안쓰는 유닛 반환
            SpawnManager.Instance.OnCalled_ReturnCamp(_spawnedUnit);
            _spawnedUnit.SetActive(false);
            _spawnedUnit = null;
        }
    }
}
