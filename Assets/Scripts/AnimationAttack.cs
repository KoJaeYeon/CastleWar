using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationAttack : MonoBehaviour,IAnimationAttack
{
    Unit _unit;
    Unit Unit
    {
        get
        {
            if (_unit == null)
            {
                _unit = transform.parent.GetComponent<Unit>();
            }
            return _unit;
        }
    }
    public void OnAttackAnimationEnd()
    {
        if (Unit != null)
        {
            _unit.OnCalled_Attack_AnimationEventAttack();
        }
    }
    public void OnSetEnemyAnimationStart()
    {
        if (Unit != null)
        {
            _unit.OnCalled_SetEnemy_AnimationEventAttack();
        }
    }
}
