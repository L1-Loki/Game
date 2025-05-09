using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _Melee : MonoBehaviour
{
    public WeaponsSciptTableObject weaponData;

    public float destroyTime;

    protected float currentDamage;
    protected float currentSpeed;
    protected float currentRecoveryTime;
    protected float currentPierce;

    void Awake()
    {
        currentDamage = weaponData.Damage;
        currentSpeed = weaponData.Speed;
        currentRecoveryTime = weaponData.RecoveryTime;
        currentPierce = weaponData.Pierce;
    }
    public float GetCurrentDamage()
    {
        return currentDamage *= FindObjectOfType<PlayerStats>().CurrentMight;
    }
    protected virtual void Start()
    {
        Destroy(gameObject, destroyTime);
    }
    protected virtual void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Enemy"))
        {
            EnemyStat enemy = col.GetComponent<EnemyStat>();
            enemy.TakeDamage(GetCurrentDamage());
        }
        else if (col.CompareTag("Prop"))
        {
            if (col.gameObject.TryGetComponent(out BreakableProps breakable))
            {
                breakable.TakeDamage(GetCurrentDamage());
            }
        }
    }
}
