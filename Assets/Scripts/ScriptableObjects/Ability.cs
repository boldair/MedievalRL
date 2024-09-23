using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewAbility", menuName = "Game/Ability")]
public class Ability : ScriptableObject
{
    public string abilityName;
    public float range;
    public float cooldown;
    public GameObject abilityEffect; // The visual effect prefab
    public float effectRadius;
    public int damage;

    public void Use(GameObject target)
    {
        // To be overridden by specific abilities
        Debug.Log(abilityName + " used on " + target.name);
    }
}