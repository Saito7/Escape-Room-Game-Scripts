using System;
using UnityEngine;

public class BookPlaceAndPickup : ObjectPlaceAndPickup, ISaveable
{
    [Header("Events")]
    [SerializeField] protected VoidEvent correctBookWasPlaced = null;
    [SerializeField] protected VoidEvent correctBookWasRemoved = null;

    [Header("Prefabs")]
    [SerializeField] private GameObject green_Book = null;
    [SerializeField] private GameObject yellow_Book = null;
    [SerializeField] private GameObject orange_Book = null;

    public override void RaiseCorrectObjectPlacedEvent()
    {
        correctBookWasPlaced.Raise();
    }

    public override void RaiseCorrectObjectRemovedEvent()
    {
        correctBookWasRemoved.Raise();
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
            if (saveData.instantiatePrefabName == green_Book.GetComponent<ItemPickup>().itemSlot.item.Name)
            {
                instantiateObject = Instantiate(green_Book, transform.position, transform.rotation, transform);
            }
            if (saveData.instantiatePrefabName == orange_Book.GetComponent<ItemPickup>().itemSlot.item.Name)
            {
                instantiateObject = Instantiate(orange_Book, transform.position, transform.rotation, transform);
            }
            if (saveData.instantiatePrefabName == yellow_Book.GetComponent<ItemPickup>().itemSlot.item.Name)
            {
                instantiateObject = Instantiate(yellow_Book, transform.position, transform.rotation, transform);
            }
            instantiateObject.transform.localScale = new Vector3(1f, 1f, 1f);
            instantiateObject.name = saveData.instantiatePrefabName;
            tempName = saveData.instantiatePrefabName;
            gameObject.GetComponent<MeshCollider>().enabled = false;
        }
    }

}
