using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class WeightPlaceAndPickup : MonoBehaviour, IInteractable,ISaveable
{
    [Header("Events")]
    [SerializeField] protected StringEvent onHoveringOverInteractable = null;
    [SerializeField] protected VoidEvent onHoveringOverInteractableEnd = null;
    [SerializeField] protected VoidEvent deselectEmpty = null;

    [Header("References")]
    [SerializeField] private InputActionReference interactAction;
    [SerializeField] private Inventory inventory = null;
    [SerializeField] private ItemType desiredItemType;

    [Header("Prefabs")]
    [SerializeField] private GameObject stonePrefab;
    [SerializeField] private GameObject blackKnightPrefab;
    [SerializeField] private GameObject blackPawnPrefab;
    [SerializeField] private GameObject whiteQueenPrefab;
    [SerializeField] private GameObject whiteBishopPrefab;
    [SerializeField] private GameObject whitePawnPrefab;
    [SerializeField] private GameObject yellowBookPrefab;
    [SerializeField] private GameObject greenBookPrefab;
    [SerializeField] private GameObject orangeBookPrefab;
    [SerializeField] private GameObject ANDGatePrefab;
    [SerializeField] private GameObject ORGatePrefab;
    [SerializeField] private GameObject smallGearPrefab;
    [SerializeField] private GameObject mediumGearPrefab;
    [SerializeField] private GameObject largeGearPrefab;
    [SerializeField] private GameObject keyPrefab;

    public bool objectHasBeenPlaced = false;
    private string tempName = null;
    private string interactText = "Place";
    private HotbarItem playerHotbarSelected = null;
    private WeighingScaleManager weighingScaleManager = null;
    private GameObject instantiateObject = null;

    private void Start()
    {
        weighingScaleManager = GetComponentInParent<WeighingScaleManager>();
    }


    public void Interact(GameObject other)
    {
        if (inventory.HasItem(playerHotbarSelected as InventoryItem) && ((desiredItemType == ItemType.AnyItem)) && (playerHotbarSelected.ItemType != ItemType.Firewood) && (playerHotbarSelected.ItemType != ItemType.PaintingDate))
        {
            //If the player has the item
            //Place
            instantiateObject = Instantiate(playerHotbarSelected.Prefab, transform.position, transform.rotation);
            if(playerHotbarSelected.ItemType == ItemType.Book)
            {
                instantiateObject.transform.position = new Vector3(transform.position.x,instantiateObject.transform.position.y - 0.0705f, transform.position.z);
            }
            if (playerHotbarSelected.ItemType == ItemType.Gear)
            {
                instantiateObject.transform.position = new Vector3(transform.position.x, instantiateObject.transform.position.y - 0.053f, transform.position.z);
            }
            if ((playerHotbarSelected.ItemType == ItemType.Chess) || (playerHotbarSelected.ItemType == ItemType.LogicGate))
            {
                instantiateObject.transform.position = new Vector3(transform.position.x, instantiateObject.transform.position.y - 0.0731f, transform.position.z);
                if(playerHotbarSelected.ItemType == ItemType.LogicGate)
                {
                    instantiateObject.transform.eulerAngles = new Vector3(90, 0, 0);
                }
            }
            
            instantiateObject.name = transform.GetSiblingIndex().ToString();
            objectHasBeenPlaced = true;
           
            //remove from inventory
            inventory.RemoveItem(new ItemSlot(playerHotbarSelected as InventoryItem, 1));
            onHoveringOverInteractableEnd.Raise();
           
            //update weighing scale manager
            if (weighingScaleManager != null)
            {
                weighingScaleManager.setItemWeight((playerHotbarSelected as InventoryItem).Weight, transform.GetSiblingIndex());
            }

            //if the player no longer has the item, then unselect the hotbarSlot
            if (!inventory.HasItem(playerHotbarSelected as InventoryItem))
            {
                deselectEmpty.Raise();
            }

            //Make object unselectable
            gameObject.layer = 0;
            GetComponent<BoxCollider>().enabled = false;
            GetComponent<Renderer>().enabled = false;
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
            if(playerHotbarSelected != null)
            {
                if (playerHotbarSelected.ItemType == ItemType.Firewood || playerHotbarSelected.ItemType == ItemType.PaintingDate)
                {
                    onHoveringOverInteractable.Raise("You Cannot Place This Here");
                }
            }
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
        Debug.Log("test");
        objectHasBeenPlaced = false;
        weighingScaleManager.setItemWeight(0, transform.GetSiblingIndex());
        gameObject.layer = 6;
        GetComponent<BoxCollider>().enabled = true;
        GetComponent<Renderer>().enabled = true;
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
                instantiatePrefabName = instantiateObject.name
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
            if (saveData.instantiatePrefabName == blackKnightPrefab.GetComponent<ItemPickup>().itemSlot.item.Name)
            {
                instantiateObject = Instantiate(blackKnightPrefab, transform.position, transform.rotation);
                instantiateObject.transform.position = new Vector3(transform.position.x, instantiateObject.transform.position.y - 0.0731f, transform.position.z);
            }
            else if (saveData.instantiatePrefabName == blackPawnPrefab.GetComponent<ItemPickup>().itemSlot.item.Name)
            {
                instantiateObject = Instantiate(blackPawnPrefab, transform.position, transform.rotation);
                instantiateObject.transform.position = new Vector3(transform.position.x, instantiateObject.transform.position.y - 0.0731f, transform.position.z);
            }
            else if (saveData.instantiatePrefabName == whiteQueenPrefab.GetComponent<ItemPickup>().itemSlot.item.Name)
            {
                instantiateObject = Instantiate(whiteQueenPrefab, transform.position, transform.rotation);
                instantiateObject.transform.position = new Vector3(transform.position.x, instantiateObject.transform.position.y - 0.0731f, transform.position.z);
            }
            else if (saveData.instantiatePrefabName == whiteBishopPrefab.GetComponent<ItemPickup>().itemSlot.item.Name)
            {
                instantiateObject = Instantiate(whiteBishopPrefab, transform.position, transform.rotation);
                instantiateObject.transform.position = new Vector3(transform.position.x, instantiateObject.transform.position.y - 0.0731f, transform.position.z);
            }
            else if (saveData.instantiatePrefabName == whitePawnPrefab.GetComponent<ItemPickup>().itemSlot.item.Name)
            {
                instantiateObject = Instantiate(whitePawnPrefab, transform.position, transform.rotation);
                instantiateObject.transform.position = new Vector3(transform.position.x, instantiateObject.transform.position.y - 0.0731f, transform.position.z);
            }
            else if (saveData.instantiatePrefabName == yellowBookPrefab.GetComponent<ItemPickup>().itemSlot.item.Name)
            {
                instantiateObject = Instantiate(yellowBookPrefab, transform.position, transform.rotation);
                instantiateObject.transform.position = new Vector3(transform.position.x, instantiateObject.transform.position.y - 0.0705f, transform.position.z);
            }
            else if (saveData.instantiatePrefabName == greenBookPrefab.GetComponent<ItemPickup>().itemSlot.item.Name)
            {
                instantiateObject = Instantiate(greenBookPrefab, transform.position, transform.rotation);
                instantiateObject.transform.position = new Vector3(transform.position.x, instantiateObject.transform.position.y - 0.0705f, transform.position.z);
            }
            else if (saveData.instantiatePrefabName == orangeBookPrefab.GetComponent<ItemPickup>().itemSlot.item.Name)
            {
                instantiateObject = Instantiate(orangeBookPrefab, transform.position, transform.rotation);
                instantiateObject.transform.position = new Vector3(transform.position.x, instantiateObject.transform.position.y - 0.0705f, transform.position.z);
            }
            else if (saveData.instantiatePrefabName == ANDGatePrefab.GetComponent<ItemPickup>().itemSlot.item.Name)
            {
                instantiateObject = Instantiate(ANDGatePrefab, transform.position, transform.rotation);
                instantiateObject.transform.position = new Vector3(transform.position.x, instantiateObject.transform.position.y - 0.0731f, transform.position.z);
                instantiateObject.transform.eulerAngles = new Vector3(90, 0, 0);
            }
            else if (saveData.instantiatePrefabName == ORGatePrefab.GetComponent<ItemPickup>().itemSlot.item.Name)
            {
                instantiateObject = Instantiate(ORGatePrefab, transform.position, transform.rotation);
                instantiateObject.transform.position = new Vector3(transform.position.x, instantiateObject.transform.position.y - 0.0731f, transform.position.z);
                instantiateObject.transform.eulerAngles = new Vector3(90, 0, 0);
            }
            else if (saveData.instantiatePrefabName == smallGearPrefab.GetComponent<ItemPickup>().itemSlot.item.Name)
            {
                instantiateObject = Instantiate(smallGearPrefab, transform.position, transform.rotation);
                instantiateObject.transform.position = new Vector3(transform.position.x, instantiateObject.transform.position.y - 0.053f, transform.position.z);
            }
            else if (saveData.instantiatePrefabName == mediumGearPrefab.GetComponent<ItemPickup>().itemSlot.item.Name)
            {
                instantiateObject = Instantiate(mediumGearPrefab, transform.position, transform.rotation);
                instantiateObject.transform.position = new Vector3(transform.position.x, instantiateObject.transform.position.y - 0.053f, transform.position.z);
            }
            else if (saveData.instantiatePrefabName == largeGearPrefab.GetComponent<ItemPickup>().itemSlot.item.Name)
            {
                instantiateObject = Instantiate(largeGearPrefab, transform.position, transform.rotation);
                instantiateObject.transform.position = new Vector3(transform.position.x, instantiateObject.transform.position.y - 0.053f, transform.position.z);
            }
            else if (saveData.instantiatePrefabName == keyPrefab.GetComponent<ItemPickup>().itemSlot.item.Name)
            {
                instantiateObject = Instantiate(keyPrefab, transform.position, transform.rotation);
            }
            else if (saveData.instantiatePrefabName == stonePrefab.GetComponent<ItemPickup>().itemSlot.item.Name)
            {
                instantiateObject = Instantiate(stonePrefab, transform.position, transform.rotation);
            }
            instantiateObject.name = saveData.instantiatePrefabName;
            tempName = saveData.instantiatePrefabName;
            gameObject.GetComponent<BoxCollider>().enabled = false;
        }
    }
}

