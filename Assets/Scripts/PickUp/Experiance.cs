using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Experiance : PickUp, ICollecTible
{

    public int expGranted;

    public void Collect()
    {
        PlayerStats player = FindObjectOfType<PlayerStats>();
        player.InCreaseExperiennce(expGranted);
    }
}
