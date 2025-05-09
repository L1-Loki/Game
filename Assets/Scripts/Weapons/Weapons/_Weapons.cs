using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _Weapons : MonoBehaviour
{
    public WeaponsSciptTableObject weaponData;
    protected Vector3 direction;
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
    // Update is called once per frame

    public void DirextionChecker(Vector3 dir)
    {
        direction = dir;

        float x = direction.x;
        float y = direction.y;

        Vector3 scale = transform.localScale;
        Vector3 rotaion = transform.rotation.eulerAngles;

        if (dir.x < 0 && dir.y == 0) // left
        {
            scale.x = scale.x * -1;
            scale.y = scale.y * -1;
        }
        else if (dir.x == 0 && dir.y < 0) // down
        {
            scale.y = scale.y * -1;
            rotaion.z = -90f;
        }
        else if (dir.x == 0 && dir.y > 0) // up
        {
            scale.x = scale.x * -1;
            rotaion.z = -90f;
        }
        else if (dir.x < 0 && dir.y < 0) // left down
        {
            scale.x = scale.x * -1;
            scale.y = scale.y * -1;
            rotaion.z = 0f;

        }
        else if (dir.x < 0 && dir.y > 0) // left up
        {
            scale.x = scale.x * -1;
            scale.y = scale.y * -1;
            rotaion.z = -90f;

        }
        else if (dir.x > 0 && dir.y < 0) // right down
        {
            rotaion.z = -90f;

        }
        else if (dir.x > 0 && dir.y > 0) // right up
        {
            rotaion.z = 0f;
        }

        transform.localScale = scale;
        transform.rotation = Quaternion.Euler(rotaion);
    }

    protected void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Enemy"))
        {
            EnemyStat enemy = col.GetComponent<EnemyStat>();
            if (enemy != null) // Kiểm tra null
            {
                enemy.TakeDamage(GetCurrentDamage());
                ReducePierce();
            }
            else
            {
                Debug.LogWarning("Không tìm thấy EnemyStart trên: " + col.name);
            }
        }
        else if (col.CompareTag("Boss")) // Thêm logic cho boss
        {
            BossController boss = col.GetComponent<BossController>();
            if (boss != null)
            {
                ReducePierce();
            }
        }
        else if (col.CompareTag("Prop"))
        {
            if (col.gameObject.TryGetComponent(out BreakableProps breakable))
            {
                breakable.TakeDamage(GetCurrentDamage());
                ReducePierce();
            }
        }
    }

    void ReducePierce()
    {
        currentPierce--;
        if (currentPierce <= 0)
        {
            Destroy(gameObject);
        }
    }
}
