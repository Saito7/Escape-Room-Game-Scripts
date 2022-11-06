using System;
using TMPro;
using UnityEngine;

public class ItemPickup : MonoBehaviour, IInteractable,ISaveable
{
    [SerializeField] protected InventoryItemEvent onHoveringOverInteractable = null;
    [SerializeField] protected VoidEvent onHoveringOverInteractableEnd = null;
    
    public ItemSlot itemSlot;
    public float desiredDistanceFromCamera = 0f;
    public bool itemPickedUp = false;


    public void Interact(GameObject other)
    {
        var itemContainer = other.GetComponent<IItemContainer>();

        if (itemContainer == null) { return; }

        if (itemContainer.AddItem(itemSlot).quantity == 0)
        {
            //get relevant pickup script and call function
            if (transform.GetComponentInParent<ObjectPlaceAndPickup>() != null)
            {
                transform.GetComponentInParent<ObjectPlaceAndPickup>().objectItemPickedUp();
            }
            else if(transform.GetComponentInParent<LogicGatePlaceAndPickup>() != null)
            {
                transform.GetComponentInParent<LogicGatePlaceAndPickup>().objectItemPickedUp();
            }
            else if(transform.parent  != null && transform.parent.parent != null)
            {
                if (transform.parent.parent.GetComponentInChildren<GearPlaceAndPickup>() != null)
                {
                    transform.parent.parent.GetComponentInChildren<GearPlaceAndPickup>().objectItemPickedUp();
                }
            }else
            {
                if(transform.name == "0")
                {
                    if (GameObject.Find("WeightPlace1").GetComponent<WeightPlaceAndPickup>().objectHasBeenPlaced)
                    {
                        GameObject.Find("WeightPlace1").GetComponent<WeightPlaceAndPickup>().objectItemPickedUp();
                    }
                }
                if (transform.name == "1")
                {
                    if (GameObject.Find("WeightPlace2").GetComponent<WeightPlaceAndPickup>().objectHasBeenPlaced)
                    {
                        GameObject.Find("WeightPlace2").GetComponent<WeightPlaceAndPickup>().objectItemPickedUp();
                    }
                }
            }
            itemPickedUp = true;
            //rathen than destroying the object, Its components are disabled so that they can be loaded later if need be.
            GetComponent<Renderer>().enabled = false;
            GetComponent<Collider>().enabled = false;
            if (GetComponentInChildren<TextMeshPro>() != null)
            {
                GetComponentInChildren<TextMeshPro>().enabled = false;
            }
            if (GetComponentInChildren<SpriteRenderer>() != null)
            {
                GetComponentInChildren<SpriteRenderer>().enabled = false;
            }
            gameObject.layer = 2; 
        }
    }

    public void OnEndHover()
    {
        onHoveringOverInteractableEnd.Raise();
    }

    public void OnStartHover()
    {
        onHoveringOverInteractable.Raise(itemSlot.item);      
    }

    [Serializable]
    private struct SaveData
    {
        public bool itemPickedUp;
    }

    public object CaptureState()
    {
        return new SaveData
        {
            itemPickedUp = itemPickedUp
        };
    }

    public void RestoreState(object state)
    {
        var saveData = (SaveData)state;
        itemPickedUp = saveData.itemPickedUp;
        if (itemPickedUp)
        {
            GetComponent<Renderer>().enabled = false;
            GetComponent<Collider>().enabled = false;
            if (GetComponentInChildren<TextMeshPro>() != null)
            {
                GetComponentInChildren<TextMeshPro>().enabled = false;
            }
            if(GetComponentInChildren<SpriteRenderer>() != null)
            {
                GetComponentInChildren<SpriteRenderer>().enabled = false;
            }
        }                      
    }
}
