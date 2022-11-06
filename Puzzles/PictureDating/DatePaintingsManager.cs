using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DatePaintingsManager : MonoBehaviour,ISaveable
{
    [Header("Events")]
    [SerializeField] protected VoidEvent puzzleCompleted = null;

    [Header("References")]
    [SerializeField] private PlayerController player;
    [SerializeField] private SavingAndLoadingManager savingAndLoading;
    [SerializeField] private LevelLoader levelLoader;
    [SerializeField] private Inventory inventory;
    [SerializeField] private HotbarItem firewoodItem;

    private int correctDatesPlacedCounter = 0;
    private bool puzzleComplete = false;

    private void Start()
    {
        if ((PlayerPrefs.GetInt("Historic") == 1))
        {
            Debug.Log("sad");
            inventory.AddItem(new ItemSlot(firewoodItem as InventoryItem, 1));
            PlayerPrefs.SetInt("Historic", 2);
            savingAndLoading.Save();
        }
    }

    public void checkForCompletion()
    {
        correctDatesPlacedCounter++;
        if (correctDatesPlacedCounter == 7)
        {
            if (!puzzleComplete)
            {
                Debug.Log("Paintings Dating puzzle complete");
                puzzleComplete = true;
                puzzleCompleted.Raise();
                savingAndLoading.Save();
                player.datingPuzzleCompleteAnim();
                Invoke("loadScene", 131/60);
            }
        }
    }

    private void loadScene()
    {
        PlayerPrefs.SetInt("currentScene", 5);
        levelLoader.LoadNextLevel(5);
    }

    public void removeCorrectDateCount()
    {
        correctDatesPlacedCounter--;
        Debug.Log("A correct date was removed");
    }

    [Serializable]
    private struct SaveData
    {
        public int correctDatesCounter;
        public bool puzzleComplete;
    }

    public object CaptureState()
    {
         return new SaveData
        {
            correctDatesCounter = correctDatesPlacedCounter,
            puzzleComplete = puzzleComplete
        };    
    }

    public void RestoreState(object state)
    {
        var saveData = (SaveData)state;

        correctDatesPlacedCounter = saveData.correctDatesCounter;
        puzzleComplete = saveData.puzzleComplete;
    }


}
