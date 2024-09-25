using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "NewCharacterData", menuName = "Game/CharacterData")]
public class CharacterData : ScriptableObject
{
    // Character stats
    public string CharacterName;

    public int MaxHp;
    public int WalkingRange;
    public int AttackPower;
    public int DefensePower;

    public RacesEnum Race;

    // List of abilities
    public List<Ability> abilities;


    public enum RacesEnum {Human, Vorvhe, Heliante}
    // Additional stats can be added here if necessary (e.g., speed, magic resist, etc.)
}