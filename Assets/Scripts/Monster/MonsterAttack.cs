using UnityEngine;

public class MonsterAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public float skillCooldown = 0.5f;     
    public float projectileSpeed = 15f;    
    public float attackRange = 5f;          
    public float projectileSpawnOffset = 1.5f; 

    [Header("References")]
    public GameObject projectilePrefab;     
    [SerializeField]
    public EnemiesScripTableObject enemyData; 

    private Transform player;               
    private float lastSkillTime;           
    private EnemyStat enemyStat;            
    private BossController bossController; 

    private void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform; 
        }
        else
        {
            Debug.LogError($"{gameObject.name}: Player with tag 'Player' not found in scene!");
            enabled = false; 
            return;
        }

        enemyStat = GetComponent<EnemyStat>();
        bossController = GetComponent<BossController>();

        if (enemyStat == null && bossController == null)
        {
            Debug.LogError($"{gameObject.name}: Requires EnemyStat or BossController component!");
            enabled = false; 
            return;
        }

        if (projectilePrefab == null)
        {
            Debug.LogError($"{gameObject.name}: projectilePrefab not assigned in Inspector!");
            enabled = false; 
        }
    }

    // Cập nhật mỗi frame: Kiểm tra khoảng cách đến người chơi và tấn công nếu trong tầm.
    private void Update()
    {
        if (player == null || projectilePrefab == null) return;

        float health = bossController != null ? bossController.currentHealth : enemyStat.currentHealth;
        if (health <= 0) return; 

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer <= attackRange) 
        {
           
            if (bossController != null)
            {
                bossController.currentState = BossController.BossState.Attack;
            }
            Attack(); 
        }
    }

    // Kiểm tra cooldown và kích hoạt bắn đạn nếu đủ thời gian.
    private void Attack()
    {
        if (Time.time - lastSkillTime >= skillCooldown)
        {
            StartCoroutine(UseSkill()); 
            lastSkillTime = Time.time;  
        }
    }

    // Sinh đạn và cấu hình hướng bay của nó.
    private System.Collections.IEnumerator UseSkill()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        Vector2 spawnPosition = (Vector2)transform.position + direction * projectileSpawnOffset;

        GameObject projectile = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.velocity = direction * projectileSpeed; 
            projectile.tag = "BossProjectile";
        }
        else
        {
            Debug.LogWarning($"{gameObject.name}: Projectile missing Rigidbody2D! Using manual movement.");
            StartCoroutine(MoveProjectileManually(projectile, direction));
        }

        BossProjectile projectileScript = projectile.GetComponent<BossProjectile>();
        if (projectileScript != null)
        {
            projectileScript.enemyData = enemyData ?? projectileScript.enemyData;
        }
        else
        {
            Debug.LogWarning($"{gameObject.name}: BossProjectile component missing on projectile prefab!");
        }

        yield return null; 
    }

    // Di chuyển đạn thủ công nếu thiếu Rigidbody2D.
    private System.Collections.IEnumerator MoveProjectileManually(GameObject projectile, Vector2 direction)
    {
        float lifetime = 5f; 
        float elapsed = 0f;  

        while (elapsed < lifetime && projectile != null)
        {
            projectile.transform.position += (Vector3)(direction * projectileSpeed * Time.deltaTime);
            elapsed += Time.deltaTime;
            yield return null; 
        }

        if (projectile != null)
        {
            Destroy(projectile);
        }
    }
}