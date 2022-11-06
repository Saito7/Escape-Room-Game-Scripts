using System;
using UnityEngine;
using UnityEngine.Audio;

public class ColouredBooksManager : MonoBehaviour,ISaveable
{
    [Header("References")]
    [SerializeField] private GameObject keyPrefab = null;
    [SerializeField] private Transform keySpawnLocation = null;
    [SerializeField] protected VoidEvent puzzleCompleted = null;

    private GameObject keyInstantation;
    private int correctBooksPlacedCounter = 0;
    private bool puzzleComplete = false;

    public void checkForCompletion()
    {
        correctBooksPlacedCounter++;
        if(correctBooksPlacedCounter == 14)
        {
            if (!puzzleComplete)
            {
                //Debug.Log("Coloured Books puzzle complete");
                //Spawn a key
                keyInstantation = Instantiate(keyPrefab, keySpawnLocation.position, keySpawnLocation.rotation);
                puzzleComplete = true;
                //Sound effect
                puzzleCompleted.Raise();
            }
        }
    }

    public void removeCorrectBookCount()
    {
        correctBooksPlacedCounter--;
        //Debug.Log("A correct book was removed");
    }

    [Serializable]
    private struct SaveData
    {
        public int correctBooksCounter;
        public bool puzzleComplete;
        public bool keyHasBeenPickedUp;

    }

    public object CaptureState()
    {
        if ((keyInstantation != null) && (keyInstantation.GetComponent<Renderer>().enabled))
        {
            return new SaveData
            {
                correctBooksCounter = correctBooksPlacedCounter,
                puzzleComplete = puzzleComplete,
                keyHasBeenPickedUp = false
            };
        }
        else
        {
            return new SaveData
            {
                correctBooksCounter = correctBooksPlacedCounter,
                puzzleComplete = puzzleComplete,
                keyHasBeenPickedUp = true
            };
        }
    }

    public void RestoreState(object state)
    {
        var saveData = (SaveData)state;

        correctBooksPlacedCounter = saveData.correctBooksCounter;
        puzzleComplete = saveData.puzzleComplete;
        if(puzzleComplete && !saveData.keyHasBeenPickedUp)
        {
            keyInstantation = Instantiate(keyPrefab, keySpawnLocation.position, keySpawnLocation.rotation);
        }
    }
}
