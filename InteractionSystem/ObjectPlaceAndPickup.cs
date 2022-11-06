using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class ObjectPlaceAndPickup : MonoBehaviour, IInteractable
{
    [Header("Events")]
    [SerializeField] protected StringEvent onHoveringOverInteractable = null;
    [SerializeField] protected VoidEvent onHoveringOverInteractableEnd = null;
    [SerializeField] protected VoidEvent deselectEmpty = null;
    [Header("References")]
    [SerializeField] private InputActionReference interactAction;
    [SerializeField] private Inventory inventory = null;
    [SerializeField] private ItemType desiredItemType;

    public GameObject instantiateObject;
    public bool objectHasBeenPlaced = false;
    public string tempName = null;
    private MeshCollider meshCollider;
    private BoxCollider boxCollider;
    private Collider collider;
    private string interactText = "Place";
    private HotbarItem playerHotbarSelected = null;

    void Start()
    {
        collider = GetComponent<Collider>();
    }

    public void Interact(GameObject other)
    {
        if(inventory.HasItem(playerHotbarSelected as InventoryItem) && ((playerHotbarSelected.ItemType == desiredItemType)|| (desiredItemType == ItemType.AnyItem)))
        {
            //If the player has the item
            //Place
            instantiateObject = Instantiate(playerHotbarSelected.Prefab, transform.position, transform.rotation, transform);
            instantiateObject.transform.localScale = new Vector3(1f, 1f, 1f);
            instantiateObject.name = playerHotbarSelected.Name;
            tempName = instantiateObject.name;
            objectHasBeenPlaced = true;
            //remove from inventory
            inventory.RemoveItem(new ItemSlot(playerHotbarSelected as InventoryItem, 1));
            onHoveringOverInteractableEnd.Raise();

            //if the correct item was placed, raise an event
            if (transform.name == instantiateObject.name)
            {
                RaiseCorrectObjectPlacedEvent();    
            }

            //if the player no longer has the item, then unselect the hotbarSlot
            if(!inventory.HasItem(playerHotbarSelected as InventoryItem))
            {
                deselectEmpty.Raise();
            }

            //Make object unselectable
            if (collider != null)
            {
                collider.enabled = false;
            }
        }      
    }

    public virtual void RaiseCorrectObjectPlacedEvent() { }

    public virtual void RaiseCorrectObjectRemovedEvent() { }

    public void OnEndHover()
    {
        if (!objectHasBeenPlaced)
        {
            onHoveringOverInteractableEnd.Raise();
        }
    }

    public void OnStartHover()
    {
        if (!objectHasBeenPlaced)
        {
            onHoveringOverInteractable.Raise(GetInteractText());
        }
    }

    private string GetInteractText()
    {
        if (inventory.HasItem(playerHotbarSelected as InventoryItem))
        {
            int bindingIndex = interactAction.action.GetBindingIndexForControl(interactAction.action.controls[0]);

            StringBuilder builder = new StringBuilder();

            builder.Append("Press ").Append(InputControlPath.ToHumanReadableString(interactAction.action.bindings[bindingIndex].effectivePath,
                InputControlPath.HumanReadableStringOptions.OmitDevice)).Append(" to ").Append(interactText + " " + playerHotbarSelected.Name);

            return builder.ToString();
        }
        else
        {
            return "";
        }
    }

    public void getPlayerSelect(HotbarItem currentlySelected)
    {
        playerHotbarSelected = currentlySelected;
    }

    public void objectItemPickedUp()
    {     
        objectHasBeenPlaced = false;
        if (transform.name == tempName)
        {
            RaiseCorrectObjectRemovedEvent();
        }
        if (collider != null)
        {
            collider.enabled = true;
        }
        instantiateObject = null;
    }

}

