using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveItems : MonoBehaviour
{
    protected PlayerStats playerStats;
    public PassiveItemsScripTableObject passiveItemsData;
    protected virtual void ApplyModifier()
    {
        playerStats = FindObjectOfType<PlayerStats>();
        ApplyModifier();
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
