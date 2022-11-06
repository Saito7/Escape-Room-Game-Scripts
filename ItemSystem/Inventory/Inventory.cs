using System;
using UnityEngine;
using UnityEngine.Events;

public class Inventory : MonoBehaviour, IItemContainer,ISaveable
{
    [SerializeField] private UnityEvent onInventoryItemsUpdated = null;

    [SerializeField] private int size = 24;

    public HotbarItem[] hotbarItems;

    private ItemSlot[] itemSlots = new ItemSlot[0];
    private int[] itemIDs = new int[24];
    private int[] itemQuantities = new int[24];
    private bool inventoryAlreadySet = false;

    public ItemSlot GetSlotByIndex(int index) => itemSlots[index];

    public void Start()
    {
        if (!inventoryAlreadySet)
        {
            itemSlots = new ItemSlot[size];
        }
    }

    public ItemSlot AddItem(ItemSlot itemSlot)
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (itemSlots[i].item != null) //if the ItemSlot contains an item
            {
                if (itemSlots[i].item == itemSlot.item)
                {
                    //if the item is the same as the item we want to add
                    int slotRemainingSpace = itemSlots[i].item.MaxStack - itemSlots[i].quantity;   //calculate if there is any quantity of item remaining.

                    if (itemSlot.quantity <= slotRemainingSpace) //if there is space to be added on
                    {
                        itemSlots[i].quantity += itemSlot.quantity; //Add the quantity of the item

                        itemSlot.quantity = 0; //previous slot is emptied

                        onInventoryItemsUpdated.Invoke(); //Update UI

                        return itemSlot; //return the itemSlot
                    }
                    else if (slotRemainingSpace > 0)
                    {
                        itemSlots[i].quantity += slotRemainingSpace; //add remaining quantity to slot

                        itemSlot.quantity -= slotRemainingSpace; //remove quantity from previous slot 
                    }
                }
            }
        }

        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (itemSlots[i].item == null) //look for an empty slot
            {
                if (itemSlot.quantity <= itemSlot.item.MaxStack)
                {
                    itemSlots[i] = itemSlot; //Add the itemSlot to the inventory

                    itemSlot.quantity = 0;  //Empty slot

                    onInventoryItemsUpdated.Invoke();  //update UI

                    return itemSlot;
                }
                else  //if quantity is greater than a max stack can have
                {
                    itemSlots[i] = new ItemSlot(itemSlot.item, itemSlot.item.MaxStack);  //create a new ItemSlot to hold remaining quantity

                    itemSlot.quantity -= itemSlot.item.MaxStack;   //remove maxstack as a maxstack was already created.
                }
            }
        }

        onInventoryItemsUpdated.Invoke(); //Update the UI

        return itemSlot;
    }

    public int GetTotalQuantity(InventoryItem item)
    {
        int totalCount = 0;
        foreach (ItemSlot itemSlot in itemSlots)
        {
            if (itemSlot.item == null) { continue; }
            //if slot is empty do not add
            if (itemSlot.item != item) { continue; }
            //if slot is not the item we are searching for do not add

            totalCount += itemSlot.quantity;
        }

        return totalCount;
    }

    public bool HasItem(InventoryItem item)
    {
        foreach (ItemSlot itemSlot in itemSlots)
        {
            if (itemSlot.item == null) { continue; }
            if (itemSlot.item != item) { continue; }

            return true; //if item is found then return true
        }

        return false; //if item is not found then return false
    }

    public void RemoveAt(int slotIndex)
    {
        //if it is out of the range then return
        if (slotIndex < 0 || slotIndex > itemSlots.Length - 1) { return; }
        itemSlots[slotIndex] = new ItemSlot();

        onInventoryItemsUpdated.Invoke();
    }

    public void RemoveItem(ItemSlot itemSlot)
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            //if an Item exists
            if (itemSlots[i].item != null)
            {
                //if the Item is the intended Item to remove
                if (itemSlots[i].item == itemSlot.item) 
                {
                    //if there are less than the quantity of items to be removed
                    if (itemSlots[i].quantity < itemSlot.quantity)
                    {
                        itemSlot.quantity -= itemSlots[i].quantity;

                        itemSlots[i] = new ItemSlot();
                    }
                    else
                    {
                        //if there are too many
                        itemSlots[i].quantity -= itemSlot.quantity;
                        //if the slot is now empty
                        if (itemSlots[i].quantity == 0)
                        {
                            itemSlots[i] = new ItemSlot();

                            onInventoryItemsUpdated.Invoke();

                            return;
                        }   

                        onInventoryItemsUpdated.Invoke();
                    }
                }
            }
        }
    }

    public void Swap(int indexOne, int indexTwo)
    {
        ItemSlot firstSlot = itemSlots[indexOne];
        ItemSlot secondSlot = itemSlots[indexTwo];

        if (firstSlot.Equals(secondSlot)) { return; }

        if (secondSlot.item != null) //if we are not dropping the item into an empty slot
        {
            if (firstSlot.item == secondSlot.item) //if these are the same item
            {
                //how much space is left in the slot
                int secondSlotRemainingSpace = secondSlot.item.MaxStack - secondSlot.quantity; 

                if (firstSlot.quantity <= secondSlotRemainingSpace) //if there is enough space
                {
                    //add the quantity all onto the second slot
                    itemSlots[indexTwo].quantity += firstSlot.quantity; 
                    //the first slot should now be empty, so the slot should be cleared.
                    itemSlots[indexOne] = new ItemSlot();

                    onInventoryItemsUpdated.Invoke(); //Update the UI

                    return;
                }
            }
        }
        //If the Items within the ItemSlots are different or maxstack is surprassed

        itemSlots[indexOne] = secondSlot;
        itemSlots[indexTwo] = firstSlot;

        onInventoryItemsUpdated.Invoke();
    }

    private void getItemIDs()
    {
        for (int i = 0; i < 24; i++)
        {
            if (itemSlots[i].quantity > 0)
            {
                itemIDs[i] = itemSlots[i].item.ItemId;
            }
            else
            {
                itemIDs[i] = -1;
            }
        }
    }
    private void getItemQuantities()
    {
        for (int i = 0; i < 24; i++)
        {
            if (itemSlots[i].quantity != 0)
            {
                itemQuantities[i] = itemSlots[i].quantity;
            }
            else
            {
                itemQuantities[i] = 0;
            }
        }
    }
    
    [Serializable]
    private struct SaveData
    {
        public int[] itemID;
        public int[] itemQuantity;
    }

    public object CaptureState()
    {
        getItemIDs();
        getItemQuantities();
        return new SaveData
        {
            itemID = itemIDs,
            itemQuantity = itemQuantities
        };
    }

    public void RestoreState(object state)
    {
        var saveData = (SaveData)state;
        bool hasAnyItem = false;
        for (int i = 0; i < 24; i++)
        {
            if(saveData.itemQuantity[i] > 0)
            {
                hasAnyItem = true;
                break;
            }
        }

        if (hasAnyItem)
        {
            itemSlots = new ItemSlot[size];

            for (int j = 0; j < 24; j++)
            {
                if (saveData.itemQuantity[j] > 0)
                {
                    foreach (HotbarItem hotbarItem in hotbarItems)
                    {
                        if (saveData.itemID[j] == hotbarItem.ItemId)
                        {
                            itemSlots[j] = new ItemSlot(hotbarItem as InventoryItem, saveData.itemQuantity[j]);
                        }
                    }
                }
                else
                {
                    itemSlots[j] = new ItemSlot();
                }
            }
            inventoryAlreadySet = true;
        }
        else
        {
            Debug.Log("No Items to load");
        }
    } 
}
