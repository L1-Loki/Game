using UnityEngine;

public class BossProjectile : MonoBehaviour
{
    public EnemiesScripTableObject enemyData;
    private float damage;

    // Khởi tạo đạn: Gán sát thương và đặt layer để xuyên qua quái.
    private void Start()
    {
        if (enemyData != null)
        {
            damage = enemyData.Damage;
        }
        else
        {
            damage = 10f; 
            Debug.LogWarning($"{gameObject.name}: enemyData is null, using default damage {damage}.");
        }

        gameObject.layer = LayerMask.NameToLayer("Projectiles");
    }


    // layer đạn xuyên qua quái, chỉ va chạm với player
    private void OnTriggerEnter2D(Collider2D collision)
    {

        PlayerStats player = collision.GetComponent<PlayerStats>();
        if (player != null)
        {
            player.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
   
    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}