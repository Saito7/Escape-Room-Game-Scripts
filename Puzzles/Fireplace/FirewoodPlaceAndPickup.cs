using System;
using UnityEngine;

public class FirewoodPlaceAndPickup : ObjectPlaceAndPickup,ISaveable
{
    [Header("Events")]
    [SerializeField] protected VoidEvent correctFirewoodWasPlaced = null;
    [SerializeField] protected VoidEvent correctFirewoodWasRemoved = null;

    [Header("Prefabs")]
    [SerializeField] private GameObject firewoodPrefab;

    public override void RaiseCorrectObjectPlacedEvent()
    {
        correctFirewoodWasPlaced.Raise();
    }

    public override void RaiseCorrectObjectRemovedEvent()
    {
        correctFirewoodWasRemoved.Raise();
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
            if (saveData.instantiatePrefabName == firewoodPrefab.GetComponent<ItemPickup>().itemSlot.item.Name)
            {
                instantiateObject = Instantiate(firewoodPrefab, transform.position, transform.rotation, transform);
            }
            instantiateObject.transform.localScale = new Vector3(1f, 1f, 1f);
            instantiateObject.name = saveData.instantiatePrefabName;
            tempName = saveData.instantiatePrefabName;
            gameObject.GetComponent<MeshCollider>().enabled = false;
        }
    }
}
