using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponsController : MonoBehaviour
{
    [Header("Weapon Settings")]
    public WeaponsSciptTableObject weaponData;
    float currentTime;
   

    protected Player player;

    protected virtual void Start()
    {
        player = FindObjectOfType<Player>();
        currentTime = weaponData.RecoveryTime;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        currentTime -= Time.deltaTime;
        if (currentTime <= 0f)
        {
            Attack();
        }
    }

    protected virtual void Attack()
    {
        currentTime = weaponData.RecoveryTime;
    }
}
