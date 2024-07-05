using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float travelTime; // 발사체가 타겟에 도달하는 데 걸리는 시간 (초)
    GameObject _targetObj;
    Transform _targetTrans;
    Vector3 _startPosition;
    float _timeElapsed;
    int _index;
    float _attackDamage;

    public void InitTargetAndShoot(Vector3 initPos, GameObject target, float attackDamage, int slotIndex)
    {
        transform.position = initPos;
        _targetObj = target;
        _targetTrans = SearchModelTrans(target);
        _startPosition = transform.position;
        _timeElapsed = 0f;
        _attackDamage = attackDamage;
        _index = slotIndex;

        // 발사체가 타겟을 바라보도록 설정
        transform.LookAt(_targetTrans);

        var trailRender = GetComponentInChildren<TrailRenderer>();
        if(trailRender != null )
        {
            trailRender.Clear();
        }

        InitAction();
        CheckDamage();
    }

    private Transform SearchModelTrans(GameObject targetObj)
    {
        for(int i = 0; i< targetObj.transform.childCount; i++)
        {
            var childTrans = targetObj.transform.GetChild(i);
            bool isModel = childTrans.GetComponent<IAnimationAttack>() != null;
            if (isModel)
            {
                return childTrans;
            }
        }
        return null;
    }

    void Update()
    {
        if (_targetTrans != null)
        {
            _timeElapsed += Time.deltaTime;
            float journeyFraction = _timeElapsed / travelTime;

            // 발사체를 타겟으로 이동
            transform.position = Vector3.Lerp(_startPosition, _targetTrans.position, journeyFraction);

            // 발사체가 타겟에 도달하면 타겟의 Transform을 null로 설정
            if(Vector3.Distance(transform.position, _targetTrans.position) < 0.2f)
            {
                gameObject.SetActive(false);
                PoolManager.Instance.ReturnPrefab(_index, gameObject);

                EndAction();
                GiveDamage();
            }
        }
    }

    public void CheckDamage()
    {       
        IAttack targetAttack = _targetObj.GetComponent<IAttack>();
        if (targetAttack != null)
        {
            if(targetAttack.OnCheckDamageDie(_attackDamage))
            {
                _targetObj.layer = LayerMask.NameToLayer("DeadUnit");
            }
        }
    }

    public virtual void GiveDamage()
    {
        IAttack targetAttack = _targetObj.GetComponent<IAttack>();
        if (targetAttack != null)
        {
            targetAttack.OnTakeDamaged(_attackDamage);
        }
        else
        {
            Debug.LogWarning("targetIsNull");
        }
    }

    //자식에서 오버로딩 할 함수
    protected virtual void InitAction()
    {

    }

    protected virtual void EndAction()
    {

    }
}
