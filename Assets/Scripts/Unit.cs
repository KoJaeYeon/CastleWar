using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public enum UnitStateEnum
{
    Idle,
    Move,
}

public class Unit : MonoBehaviour
{
    private IState _currentState;
    private readonly float _spawnTime = 1f;

    [SerializeField] float _health;
    [SerializeField] float _attackSpeed;
    [SerializeField] float _attackRange;
    [SerializeField] bool _isMelee;
    [SerializeField] float _moveSpeed;
    [SerializeField] Slider _slider;

    bool _targetChanged = false;
    GameObject _targetEnemy;

    float _attackRadius = 4f;
    float _searchRadius = 12f;

    public float MoveSpeed => _moveSpeed;
    public float SearchRadius => _searchRadius;

    public GameObject TargetEnemy
    {
        get => _targetEnemy;
        set
        {
            _targetEnemy = value;
            if (_targetEnemy != null)
            {
                TargetChanged = true;
            }
        }
    }

    public bool TargetChanged
    {
        get => _targetChanged;
        set => _targetChanged = value;
    }

    private void OnEnable()
    {
        _currentState = new UnitIdleState(this);
        _currentState.Enter();
    }

    private void OnDisable()
    {
        _currentState = null;
    }

    private void Update()
    {
        _currentState.ExecuteUpdate();
    }

    void FixedUpdate()
    {
        _currentState.ExecuteFixedUpdate();
    }

    public void OnChangeState(IState newState)
    {
        _currentState.Exit();
        _currentState = newState;
        _currentState.Enter();
    }

    public void OnValueChanged_SpawnSlider(float value)
    {
        if (_slider != null)
        {
            Debug.Log(_slider.value);
            _slider.value += value;
        }
    }

    public IEnumerator Spawn_Init()
    {
        if (_slider != null)
        {
            _slider.gameObject.SetActive(true);
            _slider.value = 0;
        }
        yield return new WaitForSeconds(_spawnTime);

        _slider?.gameObject.SetActive(false);
        OnChangeState(new UnitMoveState(this));
        yield break;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _attackRadius);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _searchRadius);

        if (_targetEnemy != null)
        {
            Gizmos.color = Color.yellow;
            Vector3 direction = (_targetEnemy.transform.position - transform.position).normalized;
            float distance = Vector3.Distance(transform.position, _targetEnemy.transform.position);
            Gizmos.DrawLine(transform.position, transform.position + direction * distance);
        }
    }
}
