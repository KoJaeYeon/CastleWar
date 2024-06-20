using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationAttack : MonoBehaviour
{
    Unit _unit;
    private void Awake()
    {
        _unit = transform.parent.GetComponent<Unit>();
    }
    public void OnAttackAnimationEnd()
    {
        if (_unit != null)
        {
            _unit.OnCalled_Attack_AnimationEventAttack();
        }
    }
    public void OnSetEnemyAnimationStart()
    {
        if (_unit != null)
        {
            _unit.OnCalled_SetEnemy_AnimationEventAttack();
        }
    }
}
