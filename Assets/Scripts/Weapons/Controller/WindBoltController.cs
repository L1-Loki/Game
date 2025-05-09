using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindBoltController : WeaponsController
{
    protected override void Start()
    {
        base.Start();
    }

    protected override void Attack()
    {
        base.Attack();
        GameObject spawnedwindbolt = Instantiate(weaponData.Prefab);
        spawnedwindbolt.transform.position = transform.position;
        spawnedwindbolt.GetComponent<_Weapons>().DirextionChecker(player.lastMove);
    }
}
