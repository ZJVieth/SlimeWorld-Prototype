using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityHandler : MonoBehaviour
{

    public GameObject[] m_abilityList;

    private List<Ability> m_abilities = new List<Ability>();

    void Start()
    {
        Activate("Slime_Jump");
        // Activate("Basic_Slash");
        // Activate("Crystal_Shot");
    }

    public List<Ability> GetAbilities()
    {
        return m_abilities;
    }

    private void Activate(string name)
    {
        // Retrieve prefab from ability list
        GameObject ability_prefab = Get(name);
        
        // Instantiate prefab
        GameObject ability_obj = Instantiate(ability_prefab, transform.position, transform.rotation);
        
        // Add ability component to list of usable abilities
        m_abilities.Add(ability_obj.GetComponent<Ability>());

        // Set as child of the Abilities gameObject
        foreach (Transform child in transform) {
            if (child.gameObject.name.Equals("Abilities")) {
                ability_obj.transform.parent = child;
            }
        }
    }

    private GameObject Get(string name)
    {
        for (int i = 0; i < m_abilityList.Length; i++) {
            GameObject ability = m_abilityList[i];
            if (ability.name == name)
                return ability;
        }
        return null;
    }

}
