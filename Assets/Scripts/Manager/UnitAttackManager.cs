﻿using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public delegate void UnitAttackDelegate(GameObject targetObject, Unit attackStartUnit);
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

        RegisterAttackMethod(1, Attack_Cannon);
        RegisterAttackMethod(4, Attack_Vanguard);
        RegisterAttackMethod(5, Attack_Archer);
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
        else
        {
            Debug.LogError($"ID :{id}의 공격 메서드가 없음");
            return null;
        }
    }

    public void Attack_Cannon(GameObject targetObject, Unit attackStartUnit)
    {
        //[TODO] 풀매니저 추가해야함
        GameObject arrow = Instantiate(new GameObject());
    }

    public void Attack_Vanguard(GameObject targetObject, Unit attackStartUnit)
    {
        IAttack targetAttack = targetObject.GetComponent<IAttack>();
        if (targetAttack != null)
        {
            targetAttack.OnTakeDamaged(attackStartUnit.AttackDamage);
        }
    }

    public void Attack_Archer(GameObject targetObject, Unit attackStartUnit)
    {
        GameObject ArrowPrefab = PoolManager.Instance.GetPrefab(attackStartUnit.SpwanSlotIndex, attackStartUnit.UnitId);
        //[TODO] 풀매니저 추가해야함
        GameObject arrow = Instantiate(new GameObject());
    }
}