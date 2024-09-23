using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewCharacterData", menuName = "Game/CharacterData")]
public class CharacterData : ScriptableObject
{
    // Character stats
    public int maxHP;
    public int walkingRange;
    public int attackPower;
    public int defensePower;

    // List of abilities
    public List<Ability> abilities;

    // Additional stats can be added here if necessary (e.g., speed, magic resist, etc.)
}