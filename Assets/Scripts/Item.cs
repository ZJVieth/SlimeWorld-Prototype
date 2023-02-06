using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Item")]
public class Item : ScriptableObject
{
    
    public string m_name = "New Item";
    public Types m_type = 0;
    public Sprite m_sprite;

    public float m_range = 1f;

    public enum Types
    {
        Misc,
        Sword
    }

}
