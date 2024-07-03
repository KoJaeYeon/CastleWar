using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttack
{
    public void OnTakeDamaged(float damage);
    public bool OnCheckDamageDie(float damage);

}
