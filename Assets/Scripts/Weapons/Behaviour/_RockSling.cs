using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _RockSling : _Melee
{
    List<GameObject> markedEnemies;
    protected override void Start()
    {
        base.Start();
        markedEnemies = new List<GameObject>();
    }
    protected override void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Enemy") && !markedEnemies.Contains(col.gameObject))
        {
            EnemyStat enemy = col.GetComponent<EnemyStat>();
            if (enemy != null) // Kiểm tra null
            {
                enemy.TakeDamage(GetCurrentDamage());
                markedEnemies.Add(col.gameObject);
            }
            else
            {
                Debug.LogWarning("Không tìm thấy EnemyStart trên: " + col.name);
            }
        }
        else if (col.CompareTag("Prop"))
        {
            if (col.gameObject.TryGetComponent(out BreakableProps breakable) && !markedEnemies.Contains(col.gameObject))
            {
                breakable.TakeDamage(GetCurrentDamage());
                markedEnemies.Add(col.gameObject);
            }
        }
    }

}
