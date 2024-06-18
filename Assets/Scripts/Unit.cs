using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public enum UnitState
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

    UnityAction<float> Change_SliderUI;

    private void Awake()
    {
        Change_SliderUI = new UnityAction<float>(OnValueChange_SliderUI);
    }
    private void OnEnable()
    {
        _slider?.onValueChanged.AddListener(Change_SliderUI);
        _currentState = new IdleState(this);
        _currentState.Enter();

    }
    private void OnDisable()
    {
        _slider?.onValueChanged.RemoveListener(Change_SliderUI);
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
            _slider.value += value/10;
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
        //OnChangeState(new MoveState(this));
        yield break;
    }

    public void OnValueChange_SliderUI(float value)
    {

    }

    
    
}
