using UnityEngine;

public class ToggleWithKeyPress : MonoBehaviour
{ 
    public bool getActiveSelf()
    {
        return gameObject.activeSelf;
    }

    public void ToggleObject()
    {
        gameObject.SetActive(!gameObject.activeSelf);      
    }

    public void SetActiveFalse()
    {
        if (gameObject.activeSelf)
        {
           // Debug.Log("switching off " + gameObject.name);
            gameObject.SetActive(false);
        }
    }

    public void SetActiveTrue()
    {
        if (!gameObject.activeSelf)
        {
            //Debug.Log("Turning On " + gameObject.name);
            gameObject.SetActive(true);
        }
    }
}
