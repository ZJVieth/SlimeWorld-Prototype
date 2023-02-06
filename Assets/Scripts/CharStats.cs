using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Property
{
    Strength,
    Agility,
    Intelligence,
    Vitality // or constitution
}

public class CharStats : MonoBehaviour
{
    // Primary Properties
    [EnumNamedArray( typeof(Property) )]
    public int[] m_base_property = new int[4];
    private int[] m_real_property = new int[4];
    private int[] m_temp_property = new int[4];

    // Linked Components
    public SpriteRenderer m_spriteRenderer;
    public float m_flashDuration = 0.25f;
    private float m_flashTimer;

    // Secondary Properties
    private int lifepoints;

    public delegate void EventDelegate();
    private EventDelegate onDamaged;
    public void SetOnDamaged(EventDelegate setEvent)
    {
        onDamaged = setEvent;
    }

    // Start is called before the first frame update
    void Start()
    {
        lifepoints = MaxLifepoints();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        for (int i = 0; i <= 3; i++) {
            m_real_property[i] = m_base_property[i] + m_temp_property[i];
        }

        if (m_flashTimer <= 0)
            m_spriteRenderer.color = Color.white;
        m_flashTimer -= Time.fixedDeltaTime;
    }

    public int MaxLifepoints()
    {
        int max_lifepoints = 4 * GetBaseProperty(Property.Vitality);

        return max_lifepoints;
    }

    public float Speed()
    {
        return GetProperty(Property.Agility);
    }

    public int GetProperty(Property prop)
    {
        return m_real_property[(int)prop];
    }

    public int GetBaseProperty(Property prop)
    {
        return m_base_property[(int)prop];
    }

    public void DealDamage(int dmg)
    {
        // Apply Damage
        lifepoints -= dmg;

        // Damage Flash
        m_flashTimer = m_flashDuration;
        m_spriteRenderer.color = Color.green;

        // Trigger OnDamaged event
        if (onDamaged != null)
            onDamaged();
    }
}
