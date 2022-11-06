using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
//base class for the UI of any itemslot

public abstract class ItemSlotUI : MonoBehaviour, IDropHandler
{
    [SerializeField] protected Image itemIconImage = null;
    
    public int SlotIndex { get; private set; }
    public abstract HotbarItem SlotItem { get; set; }

    private void OnEnable() => UpdateSlotUI();

    protected virtual void Start()       //when Inventory is first opened
    {
        SlotIndex = transform.GetSiblingIndex();
        UpdateSlotUI(); 
    }

    public abstract void OnDrop(PointerEventData eventData); //child classes will define this
    public abstract void UpdateSlotUI();
    protected virtual void EnableSlotUI(bool enable) => itemIconImage.enabled = enable;
}
