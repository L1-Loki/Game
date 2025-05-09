using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockSling : WeaponsController
{
    protected override void Start()
    {
        base.Start();
    }
    protected override void Attack()
    {
        base.Attack();
        GameObject spawnRock = Instantiate(weaponData.Prefab);
        spawnRock.transform.position = transform.position;
        spawnRock.transform.parent = transform;
    }
}
