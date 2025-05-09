using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollector : MonoBehaviour
{
    PlayerStats playerStats;
    CircleCollider2D playerCollector;
    public float pullSpeed;

    void Start()
    {
        playerStats = FindObjectOfType<PlayerStats>();
        playerCollector = GetComponent<CircleCollider2D>();
    }
    void Update()
    {
        playerCollector.radius = playerStats.CurrentMagnet;
    }
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.TryGetComponent(out ICollecTible collectible))
        {
            Rigidbody2D rb = col.gameObject.GetComponent<Rigidbody2D>();
            Vector2 direction = (transform.position - col.transform.position).normalized;
            rb.AddForce(direction * pullSpeed);

            collectible.Collect();
        }
    }
}
