using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelection : MonoBehaviour
{
    public static CharacterSelection instance;
    public CharacterScripTableObject characterData;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Debug.LogWarning("Extra" + this + "Deleted");
            Destroy(gameObject);
        }
    }
    public static CharacterScripTableObject GetData()
    {
        return instance.characterData;
    }
    public void SelectCharacter(CharacterScripTableObject character)
    {
        characterData = character;
    }
    public void DestroySingleton()
    {
        instance = null;
        Destroy(gameObject);
    }
}