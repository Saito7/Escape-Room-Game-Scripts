using System;
using UnityEngine;

public class ChessPiecePlaceAndPickup : ObjectPlaceAndPickup,ISaveable
{
    [Header("Events")]
    [SerializeField] protected VoidEvent correctChessPieceWasPlaced = null;
    [SerializeField] protected VoidEvent correctChessPieceWasRemoved = null;

    [Header("Prefabs")]
    [SerializeField] private GameObject blackPawn;
    [SerializeField] private GameObject blackKnight;
    [SerializeField] private GameObject whiteQueen;
    [SerializeField] private GameObject whitePawn;
    [SerializeField] private GameObject whiteBishop;

    public override void RaiseCorrectObjectPlacedEvent()
    {
        correctChessPieceWasPlaced.Raise();
    }

    public override void RaiseCorrectObjectRemovedEvent()
    {
        correctChessPieceWasRemoved.Raise();
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
            if (saveData.instantiatePrefabName == blackKnight.GetComponent<ItemPickup>().itemSlot.item.Name)
            {
                instantiateObject = Instantiate(blackKnight, transform.position, transform.rotation, transform);
            }
            else if (saveData.instantiatePrefabName == blackPawn.GetComponent<ItemPickup>().itemSlot.item.Name)
            {
                instantiateObject = Instantiate(blackPawn, transform.position, transform.rotation, transform);
            }
            else if (saveData.instantiatePrefabName == whiteQueen.GetComponent<ItemPickup>().itemSlot.item.Name)
            {
                instantiateObject = Instantiate(whiteQueen, transform.position, transform.rotation, transform);
            }
            else if (saveData.instantiatePrefabName == whiteBishop.GetComponent<ItemPickup>().itemSlot.item.Name)
            {
                instantiateObject = Instantiate(whiteBishop, transform.position, transform.rotation, transform);
            }
            else if (saveData.instantiatePrefabName == whitePawn.GetComponent<ItemPickup>().itemSlot.item.Name)
            {
                instantiateObject = Instantiate(whitePawn, transform.position, transform.rotation, transform);
            }
            instantiateObject.transform.localScale = new Vector3(1f, 1f, 1f);
            instantiateObject.name = saveData.instantiatePrefabName;
            tempName = saveData.instantiatePrefabName;
            gameObject.GetComponent<MeshCollider>().enabled = false;
        }
    }
}
