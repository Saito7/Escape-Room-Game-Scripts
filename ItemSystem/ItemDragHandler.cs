using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

//Handles when items are being dragged around.

[RequireComponent(typeof(CanvasGroup))]
public class ItemDragHandler : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] protected ItemSlotUI itemSlotUI = null;

    [Header("Events")]
    [SerializeField] protected HotbarItemEvent onMouseStartHoverItem = null;
    [SerializeField] protected VoidEvent onMouseEndHoverItem = null;


    private CanvasGroup canvasGroup = null;
    private Transform originalParent = null;
    private bool isHovering = false;

    public ItemSlotUI ItemSlotUI => itemSlotUI;

    private void Start() => canvasGroup = GetComponent<CanvasGroup>();

    private void OnDisable()
    {
        if (isHovering)
        {
            onMouseEndHoverItem.Raise();
            isHovering = false;
        }
    }
    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            onMouseEndHoverItem.Raise();

            originalParent = transform.parent;

            transform.SetParent(transform.parent.parent); //change parent so scaling does not affect slot

            canvasGroup.blocksRaycasts = false; //Computer ignores item that is being dragged and looks at the item below it.
        }
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            transform.position = Mouse.current.position.ReadValue(); //update position of the slot to position of the mouse
        }
    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
        //if dragged out than reset the position of the slot
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            transform.SetParent(originalParent);
            transform.localPosition = Vector3.zero;
            canvasGroup.blocksRaycasts = true;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //on hovering over the item, display a tool tip
        onMouseStartHoverItem.Raise(ItemSlotUI.SlotItem);
        isHovering = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //remove the tooltip
        onMouseEndHoverItem.Raise();
        isHovering = false;
    }
}
