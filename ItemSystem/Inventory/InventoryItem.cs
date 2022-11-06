using UnityEngine;

public abstract class InventoryItem : HotbarItem
{
    [Header("Item Data")]
    [SerializeField] [Min(1)]private int maxStack = 1;
    [SerializeField] private int weight = 0;

    public int MaxStack => maxStack;

    public int Weight => weight;
    

}
