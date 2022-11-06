using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LightPuzzleManager : MonoBehaviour,ISaveable
{
    [Header("Events")]
    [SerializeField] private VoidEvent incorrectCandle = null;
    [SerializeField] protected VoidEvent puzzleCompleted = null;

    [Header("References")]
    [SerializeField] private DungeonDoorInteract dungeonDoor = null;
    [SerializeField] private PlayerController player;
    [SerializeField] private SavingAndLoadingManager savingAndLoading;
    [SerializeField] private LevelLoader levelLoader;
    [SerializeField] private Inventory inventory;
    [SerializeField] private HotbarItem gearItem;

    private int[] candlesIgnited = new int[7];
    private int correctCandlesCount = 0;
    private int nextCandle = 0;
    private bool puzzleComplete = false;
    private bool arrayAlreadySet = false;

    private void Start()
    {
        if (!arrayAlreadySet)
        {
            for (int i = 0; i < 7; i++)
            {
                candlesIgnited[i] = -1;
            }
        }

        if ((PlayerPrefs.GetInt("Maze") == 1))
        {
            inventory.AddItem(new ItemSlot(gearItem as InventoryItem, 1));
            PlayerPrefs.SetInt("Maze", 2);
            savingAndLoading.Save();
        }
    }

    public void candleIgnited(int index)
    {
        candlesIgnited[nextCandle] = index; //add candle to the array
        nextCandle++;
        Debug.Log(index + " was ignited");
        checkOrder();
        correctCandlesCount = 0;
    } 
    
    public void candleExtinguished(int index)
    {
        candlesIgnited[nextCandle-1] = 0;     //remove candle from the array
        nextCandle--;
        Debug.Log(index + " was extinguished");
    }

    private void checkOrder()
    {
        for (int i = 0; i < 7; i++)
        {
            if (candlesIgnited[i] == i)
            {
                Debug.Log("One is correct");
                correctCandlesCount++;           //increase counter
                Debug.Log(correctCandlesCount);
            }
            else if(i >= 1)
            {
                if (candlesIgnited[i] != -1)
                {
                    ExtinguishAll(); //Reset all candles
                    return;
                }
            }          
        }
        if(correctCandlesCount == 7)
        {
            if (!puzzleComplete)
            {
                Debug.Log("Completed");
                puzzleComplete = true;
                dungeonDoor.doorUnlocked();
                //play sound effect
                puzzleCompleted.Raise();
                savingAndLoading.Save();
                player.lightPuzzleCompleteAnim();
                Invoke("loadScene", (275/60));
            }
        }
    }

    private void loadScene()
    {
        PlayerPrefs.SetInt("currentScene", 4);
        levelLoader.LoadNextLevel(4);
    }

    private void ExtinguishAll()
    {
        for (int j = 0; j < 7; j++) //empty out candlesIgnited array
        {
            candlesIgnited[j] = -1;
        }
        incorrectCandle.Raise(); //raise event for when an incorrect candle is guessed
        nextCandle = 0;
        correctCandlesCount = 0;
        Debug.Log("exiting loop");  //Reset candles
        return;
    }
    [Serializable]
    private struct SaveData
    {
        public int[] candlesIgnited;
        public int nextCandle;
        public int correctCandlesCount;
        public bool puzzleComplete;
    }

    public object CaptureState()
    {
        return new SaveData
        {
            candlesIgnited = candlesIgnited,
            nextCandle = nextCandle,
            correctCandlesCount = correctCandlesCount,
            puzzleComplete = puzzleComplete
        };
    }

    public void RestoreState(object state)
    {
        var saveData = (SaveData)state;
        nextCandle = saveData.nextCandle;
        correctCandlesCount = saveData.correctCandlesCount;
        puzzleComplete = saveData.puzzleComplete;
        if (saveData.candlesIgnited[0] != -1)
        {
            candlesIgnited = saveData.candlesIgnited;
            arrayAlreadySet = true;
        }
    }
}
