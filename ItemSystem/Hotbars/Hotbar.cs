using System;
using UnityEngine;

public class Hotbar : MonoBehaviour,ISaveable
{
    [SerializeField] private HotbarSlot[] hotbarSlots = new HotbarSlot[10];

    [Header("References")]
    [SerializeField] protected HotbarItemEvent playerHotbarSelect = null;
    [SerializeField] private Inventory inventory;

    private int[] itemIDs = new int[10];
    private HotbarItem currentlySelected = null;
    private int prevSelectIndex = -1;
    
    public void Add(HotbarItem itemToAdd)   //takes in a hotbar item
    {
        foreach(HotbarSlot hotbarSlot in hotbarSlots)   // If item is sucessfully added, then return
        {
            if (hotbarSlot.AddItem(itemToAdd)) { return; }
        }
    }

    public void Use(int index)
    {
        if (index == prevSelectIndex)
        {
            currentlySelected = hotbarSlots[index - 1].UseSlot();
            prevSelectIndex = -1;
        }
        else
        {
            if(prevSelectIndex != -1)
            {
                hotbarSlots[prevSelectIndex - 1].UseSlot();
            }
            currentlySelected = hotbarSlots[index - 1].UseSlot();
            prevSelectIndex = index;
        }
        playerHotbarSelect.Raise(currentlySelected);
    }

    public void deselectEmpty()
    {
        if(prevSelectIndex == -1)
        {
            playerHotbarSelect.Raise(null);
            return;
        }
        Use(prevSelectIndex);
    }

    private void getItemIDs()
    {
        for (int i = 0; i < 10; i++)
        {
            if(hotbarSlots[i].SlotItem != null)
            {
                itemIDs[i] = hotbarSlots[i].SlotItem.ItemId;
            }
            else
            {
                itemIDs[i] = -1;
            }
        }
    }

    [Serializable]
    private struct SaveData
    {
        public int[] itemID;
    }

    public object CaptureState()
    {
        getItemIDs();
        return new SaveData
        {
            itemID = itemIDs,
        };
    }

    public void RestoreState(object state)
    {
        var saveData = (SaveData)state;
        for (int i = 0; i < 10; i++)
        {
            if(saveData.itemID[i] != -1)
            {
                foreach (HotbarItem hotbarItem in inventory.hotbarItems)
                {
                    if(saveData.itemID[i] == hotbarItem.ItemId)
                    {
                        hotbarSlots[i].SlotItem = hotbarItem; 
                    }
                }
            }
        }
    }
}
