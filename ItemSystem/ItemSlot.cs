using UnityEngine;
using System;

[Serializable]
public struct ItemSlot //struct is easier to pass data around.
{
    public InventoryItem item;
    [Min(1)] public int quantity; 

    public ItemSlot(InventoryItem item, int quantity)
    {
        this.item = item;
        this.quantity = quantity;
    }
}
