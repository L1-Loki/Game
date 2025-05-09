using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesMove : MonoBehaviour
{
    [Header("Movement Settings")]
    public float dodgeSpeed = 8f;           // Tốc độ né
    public float dodgeCooldown = 2f;        // Thời gian chờ giữa các lần né
    public float retreatDistance = 2f;      // Khoảng cách để rút lui nếu quá gần
    public float optimalDistance = 5f;      // Khoảng cách tối ưu cho quái tầm xa
    public bool isRanged = false;           // Là quái tầm xa hay cận chiến

    Transform player;
    EnemyStat enemyStats;
    SpriteRenderer sr;              
    float lastDodgeTime;           

    void Start()
    {
        enemyStats = GetComponent<EnemyStat>();
        player = FindObjectOfType<Player>().transform;
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (player == null || enemyStats.currentHealth <= 0) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
      

        // Né tránh nếu bị tấn công và đủ cooldown
        if (CanDodge() && IsUnderAttack())
        {
            Dodge();
            return; // Ưu tiên né trước
        }

        // Xử lý di chuyển dựa trên loại quái
        if (isRanged)
        {
            HandleRangedMovement(distanceToPlayer);
        }
        else
        {
            HandleMeleeMovement(distanceToPlayer);
        }

        // Lật sprite theo hướng người chơi
        PlayerFlip();
    }

    // Di chuyển cho quái cận chiến
    void HandleMeleeMovement(float distanceToPlayer)
    {
        // Tiến gần người chơi để tấn công
        transform.position = Vector2.MoveTowards(transform.position, player.position, enemyStats.currentMovespeed * Time.deltaTime);
    }

    // Di chuyển cho quái tầm xa
    void HandleRangedMovement(float distanceToPlayer)
    {
        // Nếu quá gần, rút lui để giữ khoảng cách
        if (distanceToPlayer < retreatDistance)
        {
            Vector2 retreatDirection = (transform.position - player.position).normalized;
            transform.position += (Vector3)retreatDirection * enemyStats.currentMovespeed * Time.deltaTime;
        }
        // Nếu quá xa, tiến gần đến khoảng cách tối ưu
        else if (distanceToPlayer > optimalDistance)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.position, enemyStats.currentMovespeed * Time.deltaTime);
        }
        // Nếu trong khoảng cách tối ưu, di chuyển ngẫu nhiên để gây khó chịu
        else
        {
            Vector2 randomOffset = Random.insideUnitCircle.normalized * enemyStats.currentMovespeed * Time.deltaTime;
            transform.position += (Vector3)randomOffset;
        }
    }

    // Né tránh khi bị tấn công
    void Dodge()
    {
        Vector2 dodgeDirection = Random.insideUnitCircle.normalized;
        transform.position += (Vector3)dodgeDirection * dodgeSpeed * Time.deltaTime;
        lastDodgeTime = Time.time;
    }

    // Kiểm tra khả năng né
    bool CanDodge()
    {
        return Time.time - lastDodgeTime >= dodgeCooldown;
    }

    // Kiểm tra xem có bị tấn công không
    bool IsUnderAttack()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 2f);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                //Debug.Log(gameObject.name + " bị tấn công!");
                return true;
            }
        }
        return false;
    }

    // Lật sprite theo hướng người chơi
    void PlayerFlip()
    {
        if (player.position.x < transform.position.x)
        {
            sr.flipX = false;
        }
        else
        {
            sr.flipX = true;
        }
    }
}
