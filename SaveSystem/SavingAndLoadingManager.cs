using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SavingAndLoadingManager : MonoBehaviour
{

    void Start()
    {
        Load();
    }

    private string SavePath => $"{Application.persistentDataPath}/SaveData.txt";

    //Called when a player wants to save
    public void Save()
    {
        //Load the file and get data from the file
        //file is loaded so that only data which needs overwriting is overwritten
        var state = LoadFile();
        //the data from each saveable object is stored
        CaptureState(state);
        //this data is then saved to the file
        SaveFile(state);
    }

    public void Load()
    {
        //Load the file and get the data from the file
        var state = LoadFile();
        //load the data back into each saveable object
        RestoreState(state);
    }

    private void SaveFile(object state)
    {
        //state is the entire save data over every saveable object
        //if a file does not exist in given path then it is created
        using (var stream = File.Open(SavePath, FileMode.Create))   
        {
            var formatter = GetBinaryFormatter();
            //the data is serialized into the file as binary
            formatter.Serialize(stream, state);
        }
    }

    private Dictionary<string, object> LoadFile()
    {
        //if a file does not exist in the given savepath, then return an empty dictionary 
        //because there is no save data
        if (!File.Exists(SavePath))
        {
            return new Dictionary<string, object>();
        }

        //if the path exists then the file is opened
        using (FileStream stream = File.Open(SavePath, FileMode.Open))
        {
            var formatter = GetBinaryFormatter();
            //the data is then deserialized(changed from binary to a readable value) and passed back into the dictionary 
            return (Dictionary<string, object>)formatter.Deserialize(stream);
        }
    }

    private void CaptureState(Dictionary<string, object> state)  //Saving
    {
        //finds every gameobject containing a saveable entity component 
        foreach (var saveable in FindObjectsOfType<SaveableEntity>()) 
        {
            //for each object with said component, their data is saved into the dictionary
            //containing a string of the ID to compare on oading as well as the state (the saveable data on an object)
            state[saveable.Id] = saveable.CaptureState();
        }
    }

    private void RestoreState(Dictionary<string, object> state) //Loading
    {
        //again finds every gameobject containing a saveable entity component
        foreach( var saveable in FindObjectsOfType<SaveableEntity>())
        {
            //if the object has data saved, get the value for the related ID 
            if(state.TryGetValue(saveable.Id, out object value))
            {
                //restore the state by calling the restorestate function passing in the save data
                saveable.RestoreState(value);
            }
        }
    } 
    
    public static BinaryFormatter GetBinaryFormatter()
    {
        BinaryFormatter formatter = new BinaryFormatter();

        SurrogateSelector selector = new SurrogateSelector();

        Vector3SerializationSurrogate vector3Surrogate = new Vector3SerializationSurrogate();
        QuaternionSerializationSurrogate quaternionSurrogate = new QuaternionSerializationSurrogate();

        selector.AddSurrogate(typeof(Vector3), new StreamingContext(StreamingContextStates.All), vector3Surrogate);
        selector.AddSurrogate(typeof(Quaternion), new StreamingContext(StreamingContextStates.All), quaternionSurrogate);

        formatter.SurrogateSelector = selector;

        return formatter;
    }
}
