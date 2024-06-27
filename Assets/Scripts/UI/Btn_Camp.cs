using UnityEngine;

public class Btn_Camp : MonoBehaviour, ISelectable
{
    TouchType touchType = TouchType.NotUnit;
    bool isDown = false;
    int _index;

    private float _spawnInterval = 0.1f; // ��ȯ �ֱ� (0.1��)
    private float _lastSpawnTime = 0f;

    GameObject _spawnedUnit;

    public void SetInit(int index, int id)
    {
        _index = index;
        var unitData = DatabaseManager.Instance.OnGetUnitData(id);
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
        if (_spawnedUnit == null) return;
        if (touchType == TouchType.Unit)
        {
            //�Ⱦ��� ���� ��ȯ
            SpawnManager.Instance.OnCalled_ReturnUnit(_index, _spawnedUnit);
            _spawnedUnit.SetActive(false);
            _spawnedUnit = null;
        }
        else
        {
            //[TODO]���� ������ ���� ��ȯ
            var unit = _spawnedUnit.GetComponent<Unit>();
            unit?.StartState();
            _spawnedUnit = null;
        }
    }

    public void ExecuteUpdate(Vector3 touchPos)
    {
        if (isDown)
        {
            // ���� �巡��
            if (_spawnedUnit == null)
            {
                return;
            }

            if (touchType == TouchType.Unit)
            {
                // ���� �巡��
                touchPos.y = 0;
                _spawnedUnit.transform.position = touchPos;

                if (Time.time - _lastSpawnTime < _spawnInterval) return; // ��ȯ �ֱⰡ ���� ������ ��ȯ
                _lastSpawnTime = Time.time;

                Debug.Log("entered");
                //[TODO]���� ������ ���� ��ȯ
                var unit = _spawnedUnit.GetComponent<Unit>();
                unit?.StartState();


                //��� ���� ġȯ
                _spawnedUnit = SpawnManager.Instance.OnCalled_GetUnit(_index);
                _spawnedUnit.SetActive(true);
                touchPos.y = 0;
                _spawnedUnit.transform.position = touchPos;

                Debug.Log($"Spawn : {touchPos}");
            }
            else if (touchType == TouchType.NotUnit)
            {
                Vector3 roundedVector = new Vector3(2 * Mathf.Round(touchPos.x / 2), 0, 2 * Mathf.Round(touchPos.z / 2));
                _spawnedUnit.transform.position = roundedVector;
            }
        }
    }

    public void OnPointerExit()
    {
        if (_spawnedUnit != null)
        {
            isDown = false;

            //�Ⱦ��� ���� ��ȯ
            SpawnManager.Instance.OnCalled_ReturnUnit(_index, _spawnedUnit);
            _spawnedUnit.SetActive(false);
            _spawnedUnit = null;
        }
    }
}
