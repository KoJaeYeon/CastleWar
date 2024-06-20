using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float travelTime; // 발사체가 타겟에 도달하는 데 걸리는 시간 (초)
    Transform _targetTrans;
    Vector3 _startPosition;
    float _timeElapsed;

    public void InitTargetAndShoot(GameObject target, float attackDamage)
    {
        _targetTrans = target.transform;
        _startPosition = transform.position;
        _timeElapsed = 0f;

        // 발사체가 타겟을 바라보도록 설정
        transform.LookAt(_targetTrans);
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
            //if (journeyFraction >= 1f)
            //{
            //    _targetTrans.GetComponent<IAttack>().
            //}
        }
    }
}
