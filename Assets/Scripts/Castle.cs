using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Castle : MonoBehaviour, IAttack
{
    private IState _currentState;
    private float _spawnTime = 1f;
    Animator _animator;

    [SerializeField] int _unitId;
    [SerializeField] int _cost;
    [SerializeField] int _populaltion;
    [SerializeField] UnitType _type;
    [SerializeField] float _health;
    [SerializeField] float _maxHealth;
    [SerializeField] float _attackDamage;
    [SerializeField] float _attackSpeed;
    [SerializeField] float _attackRange;
    [SerializeField] bool _isMelee;
    [SerializeField] float _moveSpeed;
    [SerializeField] UnitType _attackType;
    [SerializeField] Slider HpSlider;
    [SerializeField] Image SpawnTimerImage;

    public MapCornerPoint MapCornerPoint { get; set; }

    GameObject _attackTargerEnemy; // 공격해야하는 적
    GameObject _targetEnemy; // 탐색되는 적
    UnitAttackDelegate _unitAttack; // 유닛마다 다르게 부여되는 공격 메서드
    Rigidbody _rigidbody;

    float _searchRadius = 12f;
    int _spawnSlotIndex = 0;
    bool _canAttack = true;
    bool _canMove = true;

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

    public int SpwanSlotIndex
    {
        get => _spawnSlotIndex;
    }
    public float MaxHealth => _maxHealth;
    public float SpawnTime => _spawnTime;
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
        }
    }

    public Animator Animator
    {
        get
        {
            if (_animator == null)
            {
                _animator = GetComponentInChildren<Animator>();
            }
            return _animator;
        }
    }

    public bool CanAttack
    {
        get => _canAttack;
        set { _canAttack = value; }
    }

    public bool CanMove
    {
        get => _canMove;
        set { _canMove = value; }
    }


    private void OnDisable()
    {
        _currentState = null;

        var SpawnTimerObject = SpawnTimerImage.transform.parent;
        SpawnTimerObject.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (_currentState != null)
        {
            _currentState.ExecuteUpdate();
        }

    }

    void FixedUpdate()
    {
        if (_currentState != null)
        {
            _currentState.ExecuteFixedUpdate();
        }

    }

    public void InitData(UnitData unitData)
    {
        _unitId = unitData.id;
        _cost = unitData.cost;
        _populaltion = unitData.Population;
        _type = unitData.unitType;
        _health = unitData.health;
        _maxHealth = unitData.health;
        _attackDamage = unitData.AttackDamage;
        _attackSpeed = unitData.AttackSpeed;
        _attackRange = unitData.AttackRange * 2;
        _attackType = unitData.AttackType;
        _searchRadius = unitData.AttackRange < 4 ? 12f : _attackRange + 2;

        if (_type == UnitType.Building)
        {
            if (_rigidbody == null)
            {
                _rigidbody = GetComponent<Rigidbody>();
            }
            _rigidbody.isKinematic = true;
            _canMove = false;
            _health = 0f;
            var SpawnTimerObject = SpawnTimerImage.transform.parent;
            SpawnTimerObject.localScale = Vector3.one * 2;

            //막사
            if (_unitId == -1)
            {
                _canAttack =false;
            }
        }
    }

    public void CheckHealthBar()
    {
        if (_health >= _maxHealth)
        {
            _health = _maxHealth;
            HpSlider.gameObject.SetActive(false);
        }
        else if (_health <= 0)
        {
            HpSlider.gameObject.SetActive(false);
        }
        else
        {
            HpSlider.gameObject.SetActive(true);
            HpSlider.value = _health / _maxHealth;
        }
    }

    public void OnTakeDamaged(float damage)
    {
        throw new System.NotImplementedException();
    }
}
