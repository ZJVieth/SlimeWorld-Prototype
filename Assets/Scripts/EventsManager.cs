using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventsManager : MonoBehaviour
{
    public static EventsManager instance;
    
    void Awake()
    {
        if (instance == null)
            instance = this;
        else {
            Destroy(this);
        }
    }
    
    public event Action InventoryChanged;

    public void OnInventoryChanged()
    {
        InventoryChanged?.Invoke();
    }
}
