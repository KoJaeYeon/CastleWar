using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationAttack_Castle : MonoBehaviour
{
    Castle _castle;

    Castle Castle
    {
        get
        {
            if (_castle == null)
            {
                _castle = transform.parent.GetComponent<Castle>();
            }
            return _castle;
        }
    }
    public void OnAttackAnimationEnd()
    {
        if (Castle != null)
        {
            _castle.OnCalled_Attack_AnimationEventAttack();
        }
    }
    public void OnSetEnemyAnimationStart()
    {
        if (Castle != null)
        {
            _castle.OnCalled_SetEnemy_AnimationEventAttack();
        }
    }
}
