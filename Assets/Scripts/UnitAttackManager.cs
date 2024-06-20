using System.Collections.Generic;
using UnityEngine;

public delegate void UnitAttackDelegate(GameObject targetObject, float attackDamage);
public class UnitAttackManager : MonoBehaviour
{
    private static UnitAttackManager _instance;

    public static UnitAttackManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameObject("UnitAttackManager").AddComponent<UnitAttackManager>();
            }
            return _instance;
        }
    }

    private Dictionary<int, UnitAttackDelegate> attackMethods = new Dictionary<int, UnitAttackDelegate>();

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        RegisterAttackMethod(1, Attack_Vanguard);
    }

    public void RegisterAttackMethod(int id, UnitAttackDelegate attackMethod)
    {
        if (!attackMethods.ContainsKey(id))
        {
            attackMethods.Add(id, attackMethod);
        }
    }

    public UnitAttackDelegate GetAttackMethod(int id)
    {
        if (attackMethods.ContainsKey(id))
        {
            return attackMethods[id];
        }
        return null;
    }

    public void Attack_Vanguard(GameObject targetObject, float attackDamage)
    {
        IAttack targetAttack = targetObject.GetComponent<IAttack>();
        if (targetAttack != null)
        {
            targetAttack.OnTakeDamaged(attackDamage);
        }
    }
}