using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Castle : MonoBehaviour, IAttack
{
    private IState _currentState;
    private float _spawnTime = 60f;
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

    [SerializeField] GameObject[] CastleTierObjects;

    public MapCornerPoint MapCornerPoint { get; set; }

    GameObject _attackTargerEnemy; // 공격해야하는 적
    GameObject _targetEnemy; // 탐색되는 적
    UnitAttackDelegate _unitAttack; // 유닛마다 다르게 부여되는 공격 메서드
    Rigidbody _rigidbody;

    float _searchRadius = 12f;
    bool _canAttack = true;
    int _index = 0;

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

        if (_rigidbody == null)
        {
            _rigidbody = GetComponent<Rigidbody>();
        }
        _rigidbody.isKinematic = true;
        _health = _maxHealth;
        var SpawnTimerObject = SpawnTimerImage.transform.parent;
        SpawnTimerObject.localScale = Vector3.one * 3;
    }

    private void ResetData()
    {
        _targetEnemy = null;
        _attackTargerEnemy = null;
    }

    public void StartState()
    {
        ResetData();

        _currentState = new CastleIdleState(this);
        _currentState.Enter();
        CheckHealthBar();
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

    public void RequestTierUp()
    {
        StartCoroutine(Tier_Up());
    }

    public IEnumerator Tier_Up()
    {
        if (SpawnTimerImage == null) yield break;
        OnChangeState(new CastleTierUpState(this));

        CastleTierObjects[_index++].SetActive(false);
        CastleTierObjects[_index].SetActive(true);

        var parentObject_Timer = SpawnTimerImage.transform.parent.gameObject;
        parentObject_Timer.SetActive(true);
        SpawnTimerImage.fillAmount = 0;

        yield return new WaitForSeconds(_spawnTime);
        parentObject_Timer.SetActive(false);
        OnChangeState(new CastleIdleState(this));
        GameManager.Instance.RequestTierUp();

        //티어업 스펙증가
        if(GameManager.Instance.Tier == 2)
        {
            _maxHealth += 3000;
            _health += 3000;            
        }
        else
        {
            _maxHealth += 3900;
            _health += 3900;
        }
        _attackDamage += 10;

        CastleTierObjects[_index++].SetActive(false);
        CastleTierObjects[_index].SetActive(true);
        yield break;
    }

    void TierUpComplete()
    {
        _attackDamage += 10;
    }

    public bool IsTagAlly()
    {
        if (tag.Equals("Ally")) return true;
        return false;
    }

    public void OnCalled_SetEnemy_AnimationEventAttack()
    {
        _attackTargerEnemy = _targetEnemy;
    }
    public void OnCalled_Attack_AnimationEventAttack()
    {
        //[TODO] 풀매니저 추가해야함
        GameObject arrow = Instantiate(new GameObject());
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
            SpawnTimerImage.fillAmount += value;
            GameManager.Instance.TierUpLeftTime = SpawnTimerImage.fillAmount;
        }
    }
    public void OnTakeDamaged(float damage)
    {
        throw new System.NotImplementedException();
    }
}
