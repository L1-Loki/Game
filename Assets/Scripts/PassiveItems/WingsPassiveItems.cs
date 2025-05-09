using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WingsPassiveItems : PassiveItems
{
    protected override void ApplyModifier()
    {
        playerStats.CurrentMoveSpeed *= 1 + passiveItemsData.Multiplier / 100f;
    }
}
