using System;
using UnityEngine;

public class DatePlaceAndPickup : ObjectPlaceAndPickup,ISaveable
{
    [Header("Events")]
    [SerializeField] protected VoidEvent correctDateWasPlaced = null;
    [SerializeField] protected VoidEvent correctDateWasRemoved = null;

    [Header("Prefabs")]
    [SerializeField] private GameObject Feb6th1918;
    [SerializeField] private GameObject Jan1st1863;
    [SerializeField] private GameObject June4th1989;
    [SerializeField] private GameObject June15th1215;
    [SerializeField] private GameObject June28th1969;
    [SerializeField] private GameObject Jan21st1989;
    [SerializeField] private GameObject Oct14th1066;

    public override void RaiseCorrectObjectPlacedEvent()
    {
        correctDateWasPlaced.Raise();
    }

    public override void RaiseCorrectObjectRemovedEvent()
    {
        correctDateWasRemoved.Raise();
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
            Destroy(instantiateObject);
            if (saveData.instantiatePrefabName == Feb6th1918.GetComponent<ItemPickup>().itemSlot.item.Name)
            {
                instantiateObject = Instantiate(Feb6th1918, transform.position, transform.rotation, transform);
            }
            else if (saveData.instantiatePrefabName == Jan1st1863.GetComponent<ItemPickup>().itemSlot.item.Name)
            {
                instantiateObject = Instantiate(Jan1st1863, transform.position, transform.rotation, transform);
            }
            else if (saveData.instantiatePrefabName == Jan21st1989.GetComponent<ItemPickup>().itemSlot.item.Name)
            {
                instantiateObject = Instantiate(Jan21st1989, transform.position, transform.rotation, transform);
            }
            else if (saveData.instantiatePrefabName == June15th1215.GetComponent<ItemPickup>().itemSlot.item.Name)
            {
                instantiateObject = Instantiate(June15th1215, transform.position, transform.rotation, transform);
            }
            else if (saveData.instantiatePrefabName == June28th1969.GetComponent<ItemPickup>().itemSlot.item.Name)
            {
                instantiateObject = Instantiate(June28th1969, transform.position, transform.rotation, transform);
            }
            else if (saveData.instantiatePrefabName == June4th1989.GetComponent<ItemPickup>().itemSlot.item.Name)
            {
                instantiateObject = Instantiate(June4th1989, transform.position, transform.rotation, transform);
            }
            else if (saveData.instantiatePrefabName == Oct14th1066.GetComponent<ItemPickup>().itemSlot.item.Name)
            {
                instantiateObject = Instantiate(Oct14th1066, transform.position, transform.rotation, transform);
            }
            instantiateObject.transform.localScale = new Vector3(1f, 1f, 1f);
            instantiateObject.name = saveData.instantiatePrefabName;
            tempName = saveData.instantiatePrefabName;
            gameObject.GetComponent<BoxCollider>().enabled = false;
        }
        else
        {
            if (instantiateObject != null)
            {
                Destroy(instantiateObject);
            }
        }
    }

}
