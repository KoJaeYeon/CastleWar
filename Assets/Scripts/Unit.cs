using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
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

    Collider[] hitColliders = new Collider[10];
    GameObject _targetEnemy;

    [SerializeField] float _health;
    [SerializeField] float _attackSpeed;
    [SerializeField] float _attackRange;
    [SerializeField] bool _isMelee;
    [SerializeField] float _moveSpeed;
    [SerializeField] Slider _slider;

    bool _targetChanged= false;


    float _attackRadius = 4f;
    float _searchRadius = 12f;

    public float MoveSpeed
    {
        get { return _moveSpeed; }
    }

    public GameObject TargetEnemy
    {
        get { return _targetEnemy; }
        set
        {
            _targetEnemy = value;
            if(_targetEnemy != null)
            {
                TargetChanged = true;
            }
        }
    }

    public bool TargetChanged
    {
        get { return _targetChanged; }
        set { _targetChanged = value; }
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
        if(_slider != null)
        {
            Debug.Log(_slider.value);
            _slider.value += value;
        }
    }

    public IEnumerator Spawn_Init()
    {
        if( _slider != null )
        {
            _slider.gameObject.SetActive(true);
            _slider.value = 0;
        }
        yield return new WaitForSeconds(_spawnTime);

        _slider?.gameObject.SetActive(false);
        OnChangeState(new UnitMoveState(this));
        yield break;
    }

    public void SearchEnemy()
    {
        if(_targetEnemy != null) // 거리가 멀어지면 null처리
        {
            if(!_targetEnemy.activeSelf)
            { 
                _targetEnemy = null;
                return; 
            }

            float distance = Vector3.Distance(transform.position, _targetEnemy.transform.position);
            if (distance > _searchRadius) _targetEnemy = null;
            return;
        }

        Vector3 origin = transform.position;
        string[] targetLayers = this.tag.Equals("Friend") ? new string[] { "EnemyGroundUnit", "EnemyAirUnit" } : new string[] { "FriendGroundUnit", "FriendAirUnit" };
        int layerMask = LayerMask.GetMask(targetLayers); // 적 유닛 레이어만 포함

        int hitCount = Physics.OverlapSphereNonAlloc(origin, _searchRadius, hitColliders, layerMask);

        float closestDistance = float.MaxValue; // 초기값을 무한대로 설정

        for (int i = 0; i < hitCount; i++)
        {
            // 자신과 다른 태그를 가진 오브젝트만 확인
            if (hitColliders[i].CompareTag(this.tag) == false)
            {
                float distance = (transform.position - hitColliders[i].transform.position).sqrMagnitude;

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    TargetEnemy = hitColliders[i].gameObject;
                }
            }
        }

        if (_targetEnemy != null)
        {
            Debug.Log("Target enemy: " + _targetEnemy.name);
        }
    }



    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1,0,0,0.2f);
        Gizmos.DrawWireSphere(transform.position, _attackRadius);
        Gizmos.color = new Color(0, 1, 0, 0.2f);
        Gizmos.DrawWireSphere(transform.position, _searchRadius);
    }
}
