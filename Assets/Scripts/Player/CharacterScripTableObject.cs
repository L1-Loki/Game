using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "CharacterScripTableObject", menuName = "ScriptableObject/character")]

public class CharacterScripTableObject : ScriptableObject
{
    [SerializeField]
    Sprite icon;
    public Sprite Icon { get => icon; private set => icon = value; }
    [SerializeField]
    new string name;
    public string Name { get => name; private set => name = value; }
    [SerializeField]
    GameObject startingWeapon;
    public GameObject StartingWeapon { get => startingWeapon; private set => startingWeapon = value; }
    [SerializeField]
    float maxHealth;
    public float MaxHealth { get => maxHealth; private set => maxHealth = value; }
    [SerializeField]
    float recoveryTime;
    public float RecoveryTime { get => recoveryTime; private set => recoveryTime = value; }
    [SerializeField]
    float moveSpeed;
    public float MoveSpeed { get => moveSpeed; private set => moveSpeed = value; }
    [SerializeField]
    float might;
    public float  Might { get => might; private set => might = value; }
    [SerializeField]
    float projectTitleSpeed;
    public float ProjectTitleSpeed { get => projectTitleSpeed; private set => projectTitleSpeed = value; }

    [SerializeField]
    float magnet;
    public float Magnet { get => magnet; private set => magnet = value; }
}
