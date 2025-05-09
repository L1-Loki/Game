using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinachPassiveItems : PassiveItems
{
    protected override void ApplyModifier()
    {
        playerStats.CurrentMight *= 1 + passiveItemsData.Multiplier / 100f;
    }
}

