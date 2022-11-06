using System;
using UnityEngine;

public class LogicGatePuzzleManager : MonoBehaviour,ISaveable
{
    //0 represents null
    //1 represents an OR gate
    //2 represents an AND gate

    [Header("References")]
    [SerializeField] private GlowManager[] circuitConnections = new GlowManager[6]; // 1 and 2 are grouped, as well as 4 and 5
    [SerializeField] private GameObject mediumGearPrefab = null;
    [SerializeField] private Transform mediumGearSpawnLocation = null;
    [SerializeField] protected VoidEvent puzzleCompleted = null;

    private int[] logicGateList = new int[7];
    private bool puzzleComplete = false;
    public GameObject mediumGearInstantiation = null;

    private void Start()
    {
        logicGateList[3] = 2;
        logicGateList[6] = 2;                            
    }

    public void updateLogicCircuit()
    {
        if (logicGateList[0] != 0)
        {
            if(logicGateList[0] == 1)
            {
                OrGate(true, false, 0);
            }
            if(logicGateList[0] == 2)
            {
                AndGate(true, false, 0);
            }
        }

        if (logicGateList[1] != 0)
        {
            if (logicGateList[1] == 1)
            {
                OrGate(true, true, 1);
            }
            if (logicGateList[1] == 2)
            {
                AndGate(true, true, 1);
            }
        }

        if(logicGateList[2] != 0)
        {
            if (logicGateList[2] == 1)
            {
                OrGate(false, false, 2);
            }
            if (logicGateList[2] == 2)
            {
                AndGate(false, false, 2);
            }
        }

        if (logicGateList[3] != 0)
        {  
            AndGate(circuitConnections[0].isGlowing, circuitConnections[1].isGlowing, 3);
        }

        if(logicGateList[4]!= 0)
        {
            if (logicGateList[4] == 1)
            {
                OrGate(circuitConnections[0].isGlowing, circuitConnections[3].isGlowing, 4);
            }
            if (logicGateList[4] == 2)
            {
                AndGate(circuitConnections[0].isGlowing, circuitConnections[3].isGlowing, 4);
            }
        }

        if(logicGateList[5]!= 0)
        {
            if (logicGateList[5] == 1)
            {
                OrGate(circuitConnections[1].isGlowing, circuitConnections[2].isGlowing, 5);
            }
            if (logicGateList[5] == 2)
            {
                AndGate(circuitConnections[1].isGlowing, circuitConnections[2].isGlowing, 5);
            }
        }

        for (int i = 0; i < 6; i++)
        {
            if (logicGateList[i] == 0)
            {
                circuitConnections[i].setGlowFalse();
            }
        }

        if (logicGateList[6] != 0)
        {
            if (circuitConnections[4].isGlowing && circuitConnections[5].isGlowing)
            {
                if (!puzzleComplete)
                {
                    Debug.Log("Complete");
                    mediumGearInstantiation = Instantiate(mediumGearPrefab, mediumGearSpawnLocation.position, mediumGearSpawnLocation.rotation);
                    puzzleComplete = true;
                    //play sound effect
                    puzzleCompleted.Raise();
                }
            }
        }
    }

    private void OrGate(bool one, bool two,int circuitindex)
    {
        if(one || two)
        {
            //Light up next line
            circuitConnections[circuitindex].setGlowTrue();            
        }
        else
        {
            circuitConnections[circuitindex].setGlowFalse();
        }
    }   

    private void AndGate(bool one, bool two, int circuitindex)
    {
        if (one && two)
        {
            //Light up next line
            circuitConnections[circuitindex].setGlowTrue();
        }
        else
        {
            circuitConnections[circuitindex].setGlowFalse();
        }
    }

    public void OrGatePlaced(int index)
    {
        logicGateList[index] = 1;
        updateLogicCircuit();
    }

    public void AndGatePlaced(int index)
    {
        logicGateList[index] = 2;
        updateLogicCircuit();
    }

    public void LogicGateRemoved(int index)
    {
        logicGateList[index] = 0;
        updateLogicCircuit();
    }

    [Serializable]
    private struct SaveData
    {
        public int[] logicGates;
        public bool puzzleComplete;
        public bool mediumGearHasBeenPickedUp;
    }

    public object CaptureState()
    {
        if ((mediumGearInstantiation != null) && (mediumGearInstantiation.GetComponent<Renderer>().enabled))
        {
            return new SaveData
            {
                logicGates = logicGateList,
                puzzleComplete = puzzleComplete,
                mediumGearHasBeenPickedUp = false
            };
        }
        else
        {
            return new SaveData
            {
                logicGates = logicGateList,
                puzzleComplete = puzzleComplete,
                mediumGearHasBeenPickedUp = true
            };
        }
    }

    public void RestoreState(object state)
    {
        var saveData = (SaveData)state;

        logicGateList = saveData.logicGates;
        puzzleComplete = saveData.puzzleComplete;
        updateLogicCircuit();
        if (puzzleComplete && !saveData.mediumGearHasBeenPickedUp)
        {
            mediumGearInstantiation = Instantiate(mediumGearPrefab, mediumGearSpawnLocation.position, mediumGearSpawnLocation.rotation);
        }
    }
}
