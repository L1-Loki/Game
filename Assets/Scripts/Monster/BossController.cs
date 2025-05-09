using UnityEngine;
using System.Collections;

public class BossController : EnemyStat
{
    public float dodgeCooldown = 2f;
    public float retreatDistance = 2f;


    private float lastDodgeTime;
    public enum BossState { Chase, Dodge, Attack, Retreat } 
    public BossState currentState; 

    void Start()
    {
        base.Awake(); 
        currentState = BossState.Chase;
    }

    void Update()
    {
        if (player == null || currentHealth <= 0) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        Debug.Log("Current State: " + currentState + " | Distance: " + distanceToPlayer + " | Health: " + currentHealth);

        // Logic chuyển trạng thái
        if (CanDodge() && IsUnderAttack())
        {
            currentState = BossState.Dodge;
        }
        else if (distanceToPlayer < retreatDistance)
        {
            currentState = BossState.Retreat;
        }
        else
        {
            currentState = BossState.Chase;
        }
        // Lưu ý: Attack được xử lý bởi BossAttack, không cần điều kiện ở đây
    }

    bool CanDodge()
    {
        return Time.time - lastDodgeTime >= dodgeCooldown;
    }

    bool IsUnderAttack()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 2f);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                //Debug.Log("Boss bị tấn công!");
                lastDodgeTime = Time.time; // Cập nhật thời gian né
                return true;
            }
        }
        return false;
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        //Debug.Log("Boss controller Taking damage: " + damage);
        //Debug.Log("Boss nhận sát thương: " + damage + " | Máu còn lại: " + currentHealth);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 3f);
    }
}