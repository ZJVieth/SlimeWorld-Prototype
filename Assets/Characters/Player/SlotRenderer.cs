using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotRenderer : MonoBehaviour
{
    // Static instantiation
    public static SlotRenderer instance;
    void Awake()
    {
        if (instance == null)
            instance = this;
        else {
            Destroy(this);
        }
    }

    // Linked Components
    public Transform m_pivot;
    public Transform m_animPivot;
    public PlayerController m_playerController;
    public SpriteRenderer m_spriteRenderer;

    void Start()
    {
        EventsManager.instance.InventoryChanged += ChangeSprite;
    }

    // 
    void FixedUpdate()
    {
        RotateSpriteTowardsMouse();
    }


    private void RotateSpriteTowardsMouse()
    {
        // Only rotate the item if the mouse is far enough from the character to avoid spastic item flipping
        if (Vector2.Distance(CamManager.instance.mouse_position_world, transform.position) > 0.4)
            m_pivot.rotation = CamManager.instance.RotationTowardsMouse(transform.position, transform.rotation, 0f);
    }

    // Change the sprite to the active item when the inventory gets changed (on InventoryChanged event)
    private void ChangeSprite()
    {
        m_spriteRenderer.sprite = Inventory.instance.GetActiveSlot().m_sprite;
    }

    public Animator PivotAnimator()
    {
        return m_animPivot.gameObject.GetComponent<Animator>();
    }
}
