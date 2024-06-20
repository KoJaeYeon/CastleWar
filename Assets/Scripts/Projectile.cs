using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float travelTime; // �߻�ü�� Ÿ�ٿ� �����ϴ� �� �ɸ��� �ð� (��)
    Transform _targetTrans;
    Vector3 _startPosition;
    float _timeElapsed;

    public void InitTargetAndShoot(GameObject target, float attackDamage)
    {
        _targetTrans = target.transform;
        _startPosition = transform.position;
        _timeElapsed = 0f;

        // �߻�ü�� Ÿ���� �ٶ󺸵��� ����
        transform.LookAt(_targetTrans);
    }

    void Update()
    {
        if (_targetTrans != null)
        {
            _timeElapsed += Time.deltaTime;
            float journeyFraction = _timeElapsed / travelTime;

            // �߻�ü�� Ÿ������ �̵�
            transform.position = Vector3.Lerp(_startPosition, _targetTrans.position, journeyFraction);

            // �߻�ü�� Ÿ�ٿ� �����ϸ� Ÿ���� Transform�� null�� ����
            //if (journeyFraction >= 1f)
            //{
            //    _targetTrans.GetComponent<IAttack>().
            //}
        }
    }
}
