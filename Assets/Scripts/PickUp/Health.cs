using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : PickUp, ICollecTible
{
    public int health;

    public void Collect()
    {
        PlayerStats playerStats = FindAnyObjectByType<PlayerStats>();
        playerStats.RestoreHealth(health);
    }
}
