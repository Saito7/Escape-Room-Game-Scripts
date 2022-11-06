using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HotbarSlot : ItemSlotUI, IDropHandler
{
    [SerializeField] private Inventory inventory = null;
    [SerializeField] private TextMeshProUGUI itemQuantityText = null;

    [SerializeField] private Image ImageComponent;
    private HotbarItem slotItem = null;
    private bool selected = false;

    private void Awake()
    {
        ImageComponent = this.GetComponent<Image>();
    }

    public override HotbarItem SlotItem
    {
        get { return slotItem; }
        set { slotItem = value; UpdateSlotUI(); }
    }

    public bool AddItem(HotbarItem itemToAdd)
    {
        if(SlotItem != null) { return false; }

        SlotItem = itemToAdd;

        return true;
    }
    
    public HotbarItem UseSlot()
    {
        UnityEngine.Color _alpha = ImageComponent.color;
        //For when using the hotbar
        if(SlotItem == null || selected) { 
            itemIconImage.rectTransform.localScale = new Vector3(1f, 1f, 1f); //GUI to give visual feedback
            _alpha.a = 140f/255f; //return to transparent
            ImageComponent.color = _alpha;
            selected = false;
            return null; 
        }
        else if (!selected)
        {
            Debug.Log("selected " + SlotItem.Name);
            selected = true;
            itemIconImage.rectTransform.localScale = new Vector3(0.8f, 0.8f, 1f); //this creates a highlight around selected item
            _alpha.a = 1f; //this increases the strength of the box around item
            ImageComponent.color = _alpha;             
            return SlotItem;
        }
        else
        {
            Debug.Log("returning null");
            return null;
        }
    }

    public override void OnDrop(PointerEventData eventData)
    {
        ItemDragHandler itemDragHandler = eventData.pointerDrag.GetComponent<ItemDragHandler>();
        if (itemDragHandler == null) { return; }

        //if the slot is an inventory slot, reference to that item rather than swap.
        InventorySlot inventorySlot = itemDragHandler.ItemSlotUI as InventorySlot;
        if (inventorySlot != null)
        {
            SlotItem = inventorySlot.ItemSlot.item;
            return;
        }

        //if the slot is a hotbar slot, then swap the items
        HotbarSlot hotbarSlot = itemDragHandler.ItemSlotUI as HotbarSlot;
        if (hotbarSlot != null)
        {
            HotbarItem oldItem = SlotItem;
            SlotItem = hotbarSlot.SlotItem;
            hotbarSlot.SlotItem = oldItem;
            return;
        }
    }

    public override void UpdateSlotUI()
    {
        //if the slot is empty, then disable it
        if(slotItem == null)
        {
            EnableSlotUI(false);
            return;
        }

        //else enable the icon
        itemIconImage.sprite = SlotItem.Icon;

        EnableSlotUI(true);

        SetItemQuantityUI();
    }

    private void SetItemQuantityUI()
    {
        if(SlotItem is InventoryItem inventoryItem)
        {
            //if the player still has item in their inventory
            if (inventory.HasItem(inventoryItem))
            {
                int quantityCount = inventory.GetTotalQuantity(inventoryItem);
                //if quantity count is greater than 1, then set to count of quanity otherwise set to a blank string
                itemQuantityText.text = quantityCount > 1 ? quantityCount.ToString() : ""; 
            }
            else
            {
                SlotItem = null;
            }
        }
        else
        {
            itemQuantityText.enabled = false;
        }
    }

    protected override void EnableSlotUI(bool enable)
    {
        base.EnableSlotUI(enable);
        itemQuantityText.enabled = enable;
    }
}

