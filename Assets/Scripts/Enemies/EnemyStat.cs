using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStat : MonoBehaviour
{
    public EnemiesScripTableObject enemyData;
    [HideInInspector]
    public float currentHealth;
    [HideInInspector]
    public float currentMovespeed;
    [HideInInspector]
    public float currentDamage;

    public float despawnDistance = 20f;
    public Transform player;
    protected virtual void Awake()
    {
        if (enemyData == null)
        {
            Debug.LogError("enemyData is not assigned on " + gameObject.name, gameObject);
            return;
        }
        currentDamage = enemyData.Damage;
        currentHealth = enemyData.MaxHealth;
        currentMovespeed = enemyData.MoveSpeed;
        //Debug.Log("Awake health: " + currentHealth);
    }

    void Start()
    {
        player = FindObjectOfType<PlayerStats>().transform;
    }
    void Update()
    {
        if (Vector2.Distance(transform.position, player.position) > despawnDistance)
        {
            ReturnEnemy();
        }
    }

    void ReturnEnemy()
    {
        EnemySpawner spawner = FindObjectOfType<EnemySpawner>();
        transform.position = player.position + spawner.relativeSpawnPositions[UnityEngine.Random.Range(0, spawner.relativeSpawnPositions.Count)].position;
    }

    public virtual void TakeDamage(float damage)
    {
        if (currentHealth <= 0)
        {
            //Debug.Log("Enemy already dead: " + gameObject.name);
            return;
        }

        //Debug.Log("Taking damage: " + damage );
        currentHealth -= damage;
        //Debug.Log("Enemy health after: " + currentHealth);
        if (currentHealth <= 0)
        {
            //Debug.Log("Health <= 0, calling Kill for: " + gameObject.name);
            Kill();
        }
    }
    public void Kill()
    {
        //Debug.Log("Enemy died: " + gameObject.name);
        Destroy(gameObject);
    }
    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            PlayerStats player = col.gameObject.GetComponent<PlayerStats>();
            player.TakeDamage(currentDamage);
        }
    }
    private void OnDestroy()
    {
        EnemySpawner spawner = FindObjectOfType<EnemySpawner>();
        if (spawner != null)
        {
            spawner.OnEnemyKilled();
        }
    }
}
