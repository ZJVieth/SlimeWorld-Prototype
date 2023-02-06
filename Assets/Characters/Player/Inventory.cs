using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory instance;

    public int m_slot_count = 3;

    public Item[] m_slot = new Item[3];

    private int m_active_slot = 0;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else {
            Destroy(this);
        }
    }

    void Start()
    {
        // Initialize Inventory dependent scripts
        EventsManager.instance.OnInventoryChanged();
    }


    public void SetActiveSlot(int set)
    {
        m_active_slot = Math.Min(set, m_slot_count - 1);
        EventsManager.instance.OnInventoryChanged();
    }

    public Item GetActiveSlot()
    {
        return m_slot[m_active_slot];
    }
}
