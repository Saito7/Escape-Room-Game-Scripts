using System;
using System.Collections.Generic;
using UnityEngine;

//This goes on any componenet that wants to be saved.

public class SaveableEntity : MonoBehaviour
{
    [SerializeField] private string id = string.Empty;

    public string Id => id;

    [ContextMenu("Generate Id")]
    private void GenerateId() => id = Guid.NewGuid().ToString();    //Generates an ID for each saveable object, so when loading data, unity knows which object the data is for.

    public object CaptureState()   //loops through all ISaveable components on the gameobject and then gets their data
    {
        var state = new Dictionary<string, object>();

        foreach(var saveable in GetComponents<ISaveable>())
        {
            state[saveable.GetType().ToString()] = saveable.CaptureState();
        }

        return state;
    }

    public void RestoreState(object state) //loops through all Isaveable components on the gameobject and then restores their data
    {
        var stateDictionary = (Dictionary<string, object>)state;

        foreach (var saveable in GetComponents<ISaveable>())
        {
            string typeName = saveable.GetType().ToString();

            if(stateDictionary.TryGetValue(typeName, out object value))
            {
                saveable.RestoreState(value);
            }
        }
    }
}
