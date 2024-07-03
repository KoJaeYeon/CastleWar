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
    [SerializeField] GameObject AttackRange_Img;

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
    public int Cost => _cost;
    public int Population => _populaltion;
    public float MaxHealth => _maxHealth;
    public float SpawnTime => _spawnTime;
    public float MoveSpeed => _moveSpeed;
    public float SearchRadius => _searchRadius;
    public float AttackRadius => _attackRange;
    public float AttackDamage => _attackDamage;
    public float TargetAnimSpeed { get; private set; }
    public UnitType AttackType => _attackType;
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
            if(_animator == null)
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
        AttackRange_Img.SetActive(true);

        var SpawnTimerObject = SpawnTimerImage.transform.parent;
        SpawnTimerObject.gameObject.SetActive(false);
    }

    private void Update()
    {
        if(_currentState != null)
        {
            _currentState.ExecuteUpdate();
        }
        
    }

    void FixedUpdate()
    {
        if(_currentState != null)
        {
            _currentState.ExecuteFixedUpdate();
        }
        
    }

    public void SetSpawnSlotIndex(int index)
    {
        _spawnSlotIndex = index;
    }

    public void InitData(UnitData unitData, int index = 0)
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
        _moveSpeed = unitData.MoveSpeed * 1.5f;
        _attackType = unitData.AttackType;
        _searchRadius = unitData.AttackRange < 4 ? 12f : _attackRange + 2;
        _spawnSlotIndex = index;

         TargetAnimSpeed = unitData.AttackAnimFrame / 60f / _attackSpeed;
        

        //캐릭터 공격범위 표시 이미지
        RectTransform rectAtkRangeImg = AttackRange_Img.GetComponent<RectTransform>();
        rectAtkRangeImg.sizeDelta = new Vector2(_attackRange*2, _attackRange*2);

        //캐릭터 체력바 색상 변경
        if(IsTagAlly() == false)
        {
            var fillImageTrans = HpSlider.transform.GetChild(1);
            var fillImage = fillImageTrans.GetComponent<Image>();
            fillImage.color = Color.red;
        }
              
        if (_type == UnitType.Building)
        {
            if (_rigidbody == null)
            {
                _rigidbody = GetComponent<Rigidbody>();
            }
            var collider = GetComponent<SphereCollider>();
            collider.radius = 2.9f;
            _rigidbody.isKinematic = true;
            _canMove = false;
            _spawnTime = 10f;
            _health = 0f;
            var SpawnTimerObject = SpawnTimerImage.transform.parent;
            SpawnTimerObject.localScale = Vector3.one * 2;

            //막사
            if(_unitId == -1)
            {
                _canAttack =false;
            }
        }
    }

    private void ResetData()
    {
        AttackRange_Img.SetActive(false);
        _health = _type == UnitType.Building ? 0 : _maxHealth;
        _targetEnemy = null;
        _attackTargerEnemy = null;
    }

    public void StartState()
    {
        ResetData();
        var SpawnTimerObject = SpawnTimerImage.transform.parent;
        SpawnTimerObject.gameObject.SetActive(true);

        _currentState = _type == UnitType.Building? new BuildingIdleState(this) : new UnitIdleState(this);
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
        else if(_health <= 0)
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
        yield return null;
        yield return null;
        OnChangeState(new UnitDeadState(this));

        yield return new WaitForSeconds(2f);
        _currentState.Exit();
    }

    public IEnumerator ProduceMana(int mana)
    {
        while(true)
        {
            yield return new WaitForSeconds(2f);
            GameManager.Instance.RequestManaProduce(mana);
        }
    }

    public void OnChangeState(IState newState)
    {
        _currentState.Exit();
        _currentState = newState;
        _currentState.Enter();
    }

    public void HandleOnRetreatState()
    {
        OnChangeState(new UnitRetreatState(this));
    }

    public void HandleOnMoveState()
    {
        OnChangeState(new UnitMoveState(this));
    }

    public bool IsTagAlly()
    {
        if (tag.Equals("Ally")) return true;
        return false;
    }

    public void OnValueChanged_SpawnSlider(float value)
    {
        if (SpawnTimerImage != null)
        {
            SpawnTimerImage.fillAmount += value;
        }
    }
    public void OnTakeDamaged(float damage)
    {
        if (_health <= 0) return;

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


    public Vector3 OnGetProjectileTrans()
    {
        ProjectileTrans ProjectileTrans = GetComponentInChildren<ProjectileTrans>();
        if(ProjectileTrans != null)
        {
            Transform trans = ProjectileTrans.transform;
            return trans.position;
        }
        else
        {
            return transform.position;
        }
    }
    public IEnumerator Spawn_Init()
    {
        if (SpawnTimerImage == null) yield break;
        var parentObject_Timer = SpawnTimerImage.transform.parent.gameObject;

        parentObject_Timer.SetActive(true);
        SpawnTimerImage.fillAmount = 0;

        yield return new WaitForSeconds(_spawnTime);
        parentObject_Timer.SetActive(false);
        if(_unitId < 0)
        {
            OnChangeState(new BuildingProduceState(this));
        }
        else
        {
            OnChangeState(new UnitMoveState(this));
        }

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
        if(_rigidbody == null)
        {
            _rigidbody = GetComponent<Rigidbody>();
        }
        if(_rigidbody.isKinematic != true)
        {
            // 충돌 시에도 속도를 0으로 유지
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
        }
    }

    public bool OnCheckDamageDie(float damage)
    {
        if (_health - damage <= 0)
        {
            return true;
        }
        else return false;
    }
}
