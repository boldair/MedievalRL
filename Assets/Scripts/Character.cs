using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public CharacterData characterData;

    // Runtime values
    private int currentHP;
    // Start is called before the first frame update
    void Start()
    {
        InitializeCharacter();
    }

    private void InitializeCharacter()
    {
        if (characterData != null)
        {
            currentHP = characterData.MaxHp;
            Debug.Log($"{characterData.CharacterName} initialized with {currentHP} HP.");
        }
    }
    public void TakeDamage(int damage)
    {
        int damageTaken = Mathf.Max(damage - characterData.DefensePower, 0);
        currentHP -= damageTaken;

        if (currentHP <= 0)
        {
            Die();
        }
    }
    public void UseAbility(int abilityIndex, GameObject target)
    {
        if (characterData != null && abilityIndex >= 0 && abilityIndex < characterData.abilities.Count)
        {
            Ability selectedAbility = characterData.abilities[abilityIndex];

            selectedAbility.Use(target);
            Debug.Log($"{characterData.CharacterName} used {selectedAbility.abilityName}.");
        }
    }
    private void Die()
    {
        Debug.Log($"{characterData.CharacterName} has died.");
        // Implement death-related behavior (e.g., removing the character from the game)
        Destroy(gameObject);
    }
}
