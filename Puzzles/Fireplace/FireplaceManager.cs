using System;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FireplaceManager : MonoBehaviour,IInteractable,ISaveable
{
    [Header("Events")]
    [SerializeField] protected StringEvent onHoveringOverInteractable = null;
    [SerializeField] protected VoidEvent onHoveringOverInteractableEnd = null;
    [SerializeField] private VoidEvent interactingWithObject = null;
    [SerializeField] private VoidEvent endInteractingWithobject = null;
    [SerializeField] protected VoidEvent puzzleCompleted = null;

    [Header("References")]
    [SerializeField] private InputActionReference interactAction;
    [SerializeField] private Transform fireplaceView = null;
    [SerializeField] private PlayerController playerController = null;
    [SerializeField] private MouseLook mouseLookScript = null;
    [SerializeField] private Image crosshair = null;
    [SerializeField] private SavingAndLoadingManager savingAndLoadingManager = null;
    [SerializeField] private GameObject flames = null;
    [SerializeField] private LevelLoader levelLoader;

    [Header("Settings")]
    [SerializeField] private float transitionSpeed;

    private HotbarItem playerHotbarSelected = null;
    private string totalInteractText = null;
    private string interactText = "Interact";
    private bool lerping = false;
    private bool interacting = false;
    private bool puzzleComplete = false;
    private bool fireIgnited = false;
    private int correctLogsCounter = 0;

    private void LateUpdate()
    {
        if (lerping)
        {
            MoveCameraAboveBoard();
        }
        if (Keyboard.current.escapeKey.wasPressedThisFrame && interacting && !lerping)
        {
            crosshair.enabled = true;
            Cursor.lockState = CursorLockMode.Locked;
            mouseLookScript.enabled = true;
            playerController.enabled = true;
            gameObject.layer = 6;
            interacting = false;
            endInteractingWithobject.Raise();
        }
    }

    public void Interact(GameObject other)
    {
        if (!interacting && !puzzleComplete)
        {
            mouseLookScript.enabled = false;
            playerController.enabled = false;
            gameObject.layer = 0;
            lerping = true;
            interacting = true;
            interactingWithObject.Raise();
        }
        else if (puzzleComplete)
        {
            totalInteractText = GetInteractText();
            if (playerHotbarSelected != null)
            {
                if (playerHotbarSelected.Name == "Lighter" && !fireIgnited)
                {
                    flames.SetActive(true);
                    puzzleComplete = true;
                    puzzleCompleted.Raise();
                    fireIgnited = true;
                    Invoke("gameComplete", 5f);
                }
            }
            else
            {
                totalInteractText = "Lighter Required";
            }
            onHoveringOverInteractable.Raise(totalInteractText);
        }
    }

    public void gameComplete()
    {
        Cursor.lockState = CursorLockMode.Confined;
        savingAndLoadingManager.Save();
        levelLoader.LoadNextLevel(6);
    }

    public void OnEndHover()
    {
        onHoveringOverInteractableEnd.Raise();
    }

    public void OnStartHover()
    {
        if (!puzzleComplete)
        {
            interactText = "Interact";
            onHoveringOverInteractable.Raise(GetInteractText());
        }
        else if (playerHotbarSelected != null)
        {
            if (playerHotbarSelected.Name == "Lighter" && !fireIgnited)
            {
                interactText = "Ignite";
            }
            onHoveringOverInteractable.Raise(GetInteractText());
        }
        else
        {
            onHoveringOverInteractable.Raise("Lighter Required");
        }
    }

    public void getPlayerSelect(HotbarItem currentlySelected)
    {
        playerHotbarSelected = currentlySelected;
    }

    private string GetInteractText()
    {
        int bindingIndex = interactAction.action.GetBindingIndexForControl(interactAction.action.controls[0]);

        StringBuilder builder = new StringBuilder();

        builder.Append("Press ").Append(InputControlPath.ToHumanReadableString(interactAction.action.bindings[bindingIndex].effectivePath,
            InputControlPath.HumanReadableStringOptions.OmitDevice)).Append(" to ").Append(interactText + " with " + gameObject.name);

        return builder.ToString();
    }

    private void MoveCameraAboveBoard()
    {
        //Lerp position of camera
        Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, fireplaceView.position, Time.deltaTime * transitionSpeed);

        Vector3 currentAngle = new Vector3(
           Mathf.LerpAngle(Camera.main.transform.rotation.eulerAngles.x, fireplaceView.transform.rotation.eulerAngles.x, Time.deltaTime * transitionSpeed),
           Mathf.LerpAngle(Camera.main.transform.rotation.eulerAngles.y, fireplaceView.transform.rotation.eulerAngles.y, Time.deltaTime * transitionSpeed),
           Mathf.LerpAngle(Camera.main.transform.rotation.eulerAngles.z, fireplaceView.transform.rotation.eulerAngles.z, Time.deltaTime * transitionSpeed));

        Camera.main.transform.eulerAngles = currentAngle;

        if (Vector3.Distance(Camera.main.transform.position, fireplaceView.transform.position) < 0.001f)
        {
            lerping = false;
            Camera.main.transform.position = fireplaceView.transform.position;
            Camera.main.transform.eulerAngles = fireplaceView.transform.eulerAngles;
            crosshair.enabled = false;
            Cursor.lockState = CursorLockMode.Confined;
        }
    }
    public void checkForCompletion()
    {
        correctLogsCounter++;
        if (correctLogsCounter == 4)
        {
            if (!puzzleComplete)
            {
                Debug.Log("Firelog complete");
                puzzleComplete = true;
                //play sound effect
                puzzleCompleted.Raise();
                crosshair.enabled = true;
                Cursor.lockState = CursorLockMode.Locked;
                mouseLookScript.enabled = true;
                playerController.enabled = true;
                interacting = false;
                endInteractingWithobject.Raise();
                gameObject.layer = 6;
            }
        }
    }

    public void removeCorrectLogCount()
    {
        correctLogsCounter--;
        Debug.Log("A correct log was removed");
    }

    [Serializable]
    private struct SaveData
    {
        public int correctLogCount;
        public bool puzzleComplete;
        public bool fireIgnited;
    }

    public object CaptureState()
    {
        return new SaveData
        {
            correctLogCount = correctLogsCounter,
            puzzleComplete = puzzleComplete,
            fireIgnited = fireIgnited
        };
    }

    public void RestoreState(object state)
    {
        var saveData = (SaveData)state;

        puzzleComplete = saveData.puzzleComplete;
        correctLogsCounter = saveData.correctLogCount;
        fireIgnited = saveData.fireIgnited;
        if (puzzleComplete && fireIgnited)
        {
            Cursor.lockState = CursorLockMode.Confined;
            levelLoader.LoadNextLevel(6);
        }
    }

}
