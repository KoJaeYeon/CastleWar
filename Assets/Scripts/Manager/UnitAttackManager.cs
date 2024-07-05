using System.Collections.Generic;
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

        RegisterAttackMethod(1, Attack_Projectile);
        RegisterAttackMethod(3, Attack_Melee);
        RegisterAttackMethod(4, Attack_Melee);
        RegisterAttackMethod(5, Attack_Projectile);
        RegisterAttackMethod(13, Attack_Projectile);
        RegisterAttackMethod(18, Attack_Projectile);
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

    //public void Attack_Cannon(GameObject targetObject, Unit attackStartUnit)
    //{
    //    GameObject projectilePrefab = PoolManager.Instance.GetPrefab(attackStartUnit.SpwanSlotIndex, attackStartUnit.UnitId);
    //    projectilePrefab.SetActive(true);
    //    Projectile projectile = projectilePrefab.GetComponent<Projectile>();
    //    Vector3 startPos = attackStartUnit.OnGetProjectileTrans();
    //    projectile.InitTargetAndShoot(startPos, targetObject, attackStartUnit.AttackDamage, attackStartUnit.SpwanSlotIndex);
    //}

    public void Attack_Melee(GameObject targetObject, Unit attackStartUnit)
    {
        IAttack targetAttack = targetObject.GetComponent<IAttack>();
        if (targetAttack != null)
        {
            targetAttack.OnTakeDamaged(attackStartUnit.AttackDamage);
        }
    }

    //public void Attack_Archer(GameObject targetObject, Unit attackStartUnit)
    //{
    //    GameObject arrowPrefab = PoolManager.Instance.GetPrefab(attackStartUnit.SpwanSlotIndex, attackStartUnit.UnitId);
    //    arrowPrefab.SetActive(true);
    //    Projectile projectile = arrowPrefab.GetComponent<Projectile>();
    //    Vector3 startPos = attackStartUnit.OnGetProjectileTrans();
    //    projectile.InitTargetAndShoot(startPos, targetObject, attackStartUnit.AttackDamage, attackStartUnit.SpwanSlotIndex);
    //}

    public void Attack_Projectile(GameObject targetObject, Unit attackStartUnit)
    {
        GameObject arrowPrefab = PoolManager.Instance.GetPrefab(attackStartUnit.SpwanSlotIndex, attackStartUnit.UnitId);
        arrowPrefab.SetActive(true);
        Projectile projectile = arrowPrefab.GetComponent<Projectile>();
        Vector3 startPos = attackStartUnit.OnGetProjectileTrans();
        projectile.InitTargetAndShoot(startPos, targetObject, attackStartUnit.AttackDamage, attackStartUnit.SpwanSlotIndex);
    }

    //public void Attack_Owl(GameObject targetObject, Unit attackStartUnit)
    //{
    //    GameObject arrowPrefab = PoolManager.Instance.GetPrefab(attackStartUnit.SpwanSlotIndex, attackStartUnit.UnitId);
    //    arrowPrefab.SetActive(true);
    //    Projectile projectile = arrowPrefab.GetComponent<Projectile>();
    //    Vector3 startPos = attackStartUnit.OnGetProjectileTrans();
    //    projectile.InitTargetAndShoot(startPos, targetObject, attackStartUnit.AttackDamage, attackStartUnit.SpwanSlotIndex);
    //}
}