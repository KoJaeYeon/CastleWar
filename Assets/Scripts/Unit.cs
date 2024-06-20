using System.Collections;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public enum UnitStateEnum
{
    Idle,
    Move,
}

public class Unit : MonoBehaviour, IAttack
{
    private IState _currentState;
    private readonly float _spawnTime = 1f;
    Animator _animator;

    [SerializeField] int _unitId;
    [SerializeField] float _health;
    [SerializeField] float _attackDamage;
    [SerializeField] float _attackSpeed;
    [SerializeField] float _attackRange;
    [SerializeField] bool _isMelee;
    [SerializeField] float _moveSpeed;
    [SerializeField] Slider _slider;

    bool _targetChanged = false;
    GameObject _targetEnemy;
    UnitAttackDelegate _unitAttack;

    float _searchRadius = 12f;

    public int UnitId
    {
        get => _unitId;
        set
        {
            _unitId = value;
        }
    }
    public float MoveSpeed => _moveSpeed;
    public float SearchRadius => _searchRadius;
    public float AttackRadius => _attackRange;
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

    public Animator Animator
    {
        get => _animator;
    }
    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
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
    public void OnTakeDamaged(float damage)
    {
        _health -= damage;
    }

    public void OnCalledAnimationEventAttack()
    {
        if (_unitAttack == null)
        {
            _unitAttack = UnitAttackManager.Instance.GetAttackMethod(_unitId);
        }
        _unitAttack.Invoke(_targetEnemy,_attackDamage);
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
        Gizmos.DrawWireSphere(transform.position, _attackRange);
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
