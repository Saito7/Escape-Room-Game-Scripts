using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System;

public class LogicGatePlaceAndPickup : MonoBehaviour, IInteractable,ISaveable
{
    [Header("Events")]
    [SerializeField] protected StringEvent onHoveringOverInteractable = null;
    [SerializeField] protected VoidEvent onHoveringOverInteractableEnd = null;
    [SerializeField] protected VoidEvent deselectEmpty = null;
    [SerializeField] protected IntEvent ORGatePlaced = null;
    [SerializeField] protected IntEvent ANDGatePlaced = null;
    [SerializeField] protected IntEvent LogicGateRemoved = null;

    [Header("References")]
    [SerializeField] private InputActionReference interactAction;
    [SerializeField] private Inventory inventory = null;
    [SerializeField] private ItemType desiredItemType;

    [Header("Prefabs")]
    [SerializeField] private GameObject ANDGate = null;
    [SerializeField] private GameObject ORGate = null;

    private GameObject instantiateObject = null;
    private bool objectHasBeenPlaced = false;
    private string interactText = "Place";
    private string tempName = null;
    private HotbarItem playerHotbarSelected = null;

    public void Interact(GameObject other)
    {
        if (inventory.HasItem(playerHotbarSelected as InventoryItem) && (playerHotbarSelected.ItemType == desiredItemType))
        {
            //If the player has the item, Place
            instantiateObject = Instantiate(playerHotbarSelected.Prefab, transform.position, transform.rotation, transform);
            instantiateObject.transform.localScale = new Vector3(1f, 1f, 1f);
            instantiateObject.name = playerHotbarSelected.Name;
            tempName = instantiateObject.name;
            objectHasBeenPlaced = true;
            //remove from inventory
            inventory.RemoveItem(new ItemSlot(playerHotbarSelected as InventoryItem, 1));
            onHoveringOverInteractableEnd.Raise();

            //Act according to the gate placed
            if (instantiateObject.name == "Or Gate")
            {
                ORGatePlaced.Raise(transform.GetSiblingIndex());       
            }
            else if(instantiateObject.name == "And Gate")
            {
                ANDGatePlaced.Raise(transform.GetSiblingIndex());
            }

            //if the player no longer has the item, then unselect the hotbarSlot
            if (!inventory.HasItem(playerHotbarSelected as InventoryItem))
            {
                deselectEmpty.Raise();
            }

            //Make object unselectable
            gameObject.layer = 0;
        }
    }

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
        gameObject.layer = 6;
        LogicGateRemoved.Raise(transform.GetSiblingIndex());
    }

    [Serializable]
    private struct SaveData
    {
        public bool objectHasBeenPlaced;
        public string instantiatePrefabName;
    }

    public object CaptureState()
    {
        if (instantiateObject != null)
        {
            return new SaveData
            {
                objectHasBeenPlaced = objectHasBeenPlaced,
                instantiatePrefabName = tempName
            };
        }
        else
        {
            return new SaveData
            {
                objectHasBeenPlaced = objectHasBeenPlaced
            };
        }
    }

    public void RestoreState(object state)
    {
        var saveData = (SaveData)state;

        objectHasBeenPlaced = saveData.objectHasBeenPlaced;
        if (objectHasBeenPlaced)
        {
            if (saveData.instantiatePrefabName == ANDGate.GetComponent<ItemPickup>().itemSlot.item.Name)
            {
                instantiateObject = Instantiate(ANDGate, transform.position, transform.rotation, transform);
            }
            else if (saveData.instantiatePrefabName == ORGate.GetComponent<ItemPickup>().itemSlot.item.Name)
            {
                instantiateObject = Instantiate(ORGate, transform.position, transform.rotation, transform);
            }
            instantiateObject.transform.localScale = new Vector3(1f, 1f, 1f);
            instantiateObject.name = saveData.instantiatePrefabName;
            tempName = saveData.instantiatePrefabName;
            gameObject.GetComponent<MeshCollider>().enabled = false;
        }
    }

}


