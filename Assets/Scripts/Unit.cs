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
    [SerializeField] float _maxHealth;
    [SerializeField] float _attackDamage;
    [SerializeField] float _attackSpeed;
    [SerializeField] float _attackRange;
    [SerializeField] bool _isMelee;
    [SerializeField] float _moveSpeed;
    [SerializeField] Slider HpSlider;
    [SerializeField] Image SpawnTimerImage;

    public MapCornerPoint MapCornerPoint { get; set; }

    bool _targetChanged = false; // 가까운 유닛이 변경 되었을 때 경계검사 한번(Astar, Ray 포함)
    GameObject _attackTargerEnemy; // 공격해야하는 적
    GameObject _targetEnemy; // 탐색되는 적
    UnitAttackDelegate _unitAttack;
    Rigidbody _rigidbody;

    float _searchRadius = 12f;
    bool _unitDied = false;

    public float Helath
    {
        get => _health;
        set
        {
            _health = value;
            CheckHealthBar();
        }
    }
    public int UnitId
    {
        get => _unitId;
        set
        {
            _unitId = value;
        }
    }
    public bool UnitDied
    {
        get => _unitDied;
        private set
        {
            _unitDied = value;
        }
    }

    
    public float MoveSpeed => _moveSpeed;
    public float SearchRadius => _searchRadius;
    public float AttackRadius => _attackRange;
    public float AttackDamage => _attackDamage;
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
        _rigidbody = GetComponent<Rigidbody>();
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
        CheckHealthBar();
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

    public void CheckHealthBar()
    {
        if (_health == _maxHealth)
        {
            HpSlider.gameObject.SetActive(false);
        }
        else
        {
            HpSlider.gameObject.SetActive(true);
            HpSlider.value = _health / _maxHealth;
        }
    }

    public IEnumerator UnitDieCoroutine()
    {
        _unitDied = true;
        yield return null;
        yield return null;
        gameObject.SetActive(false);
    }

    public void OnChangeState(IState newState)
    {
        _currentState.Exit();
        _currentState = newState;
        _currentState.Enter();
    }

    public void OnValueChanged_SpawnSlider(float value)
    {
        if (SpawnTimerImage != null)
        {
            Debug.Log(SpawnTimerImage.fillAmount);
            SpawnTimerImage.fillAmount += value;
        }
    }
    public void OnTakeDamaged(float damage)
    {
        if (_unitDied) return;
        Helath -= damage;
        if(_health <= 0)
        {
            StartCoroutine(UnitDieCoroutine());
        }
    }

    public void OnCalled_SetEnemy_AnimationEventAttack()
    {
        _attackTargerEnemy = _targetEnemy;
    }
    public void OnCalled_Attack_AnimationEventAttack()
    {
        if (_unitAttack == null)
        {
            _unitAttack = UnitAttackManager.Instance.GetAttackMethod(_unitId);
        }

        if(_attackTargerEnemy != null)
        {
            _unitAttack.Invoke(_attackTargerEnemy, this);
        }
        
    }

    public IEnumerator Spawn_Init()
    {
        if (SpawnTimerImage != null)
        {
            SpawnTimerImage.transform.parent.gameObject.SetActive(true);
            SpawnTimerImage.fillAmount = 0;
        }
        yield return new WaitForSeconds(_spawnTime);

        SpawnTimerImage?.transform.parent.gameObject.SetActive(false);
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

    public void OnCollisionExit(Collision collision)
    {
        // 충돌 시에도 속도를 0으로 유지
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;
    }
}
