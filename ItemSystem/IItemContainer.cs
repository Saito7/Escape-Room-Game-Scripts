public interface IItemContainer
{
    //This is for what an itemcontainer needs
    ItemSlot AddItem(ItemSlot itemSlot);
    void RemoveItem(ItemSlot itemSlot);
    void RemoveAt(int slotIndex); //remove an item at a certain index
    void Swap(int indexOne, int indexTwo); //swaping items
    bool HasItem(InventoryItem item);//check if you have a certain item
    int GetTotalQuantity(InventoryItem item); //how many of an item you have
}
