using UnityEngine.InputSystem;
using UnityEngine;
using System.Text;
using UnityEngine.UI;
using System;

public class ChessPuzzleManager : MonoBehaviour, IInteractable,ISaveable
{
    [Header("Events")]
    [SerializeField] protected StringEvent onHoveringOverInteractable = null;
    [SerializeField] protected VoidEvent onHoveringOverInteractableEnd = null;
    [SerializeField] private VoidEvent interactingWithObject = null;
    [SerializeField] private VoidEvent endInteractingWithobject = null;
    [SerializeField] protected VoidEvent puzzleCompleted = null;

    [Header("References")]
    [SerializeField] private InputActionReference interactAction;
    [SerializeField] private Transform chessBoardView = null;
    [SerializeField] private PlayerController playerController = null;
    [SerializeField] private MouseLook mouseLookScript = null;
    [SerializeField] private Image crosshair = null;
    [SerializeField] private GameObject smallGearPrefab = null;
    [SerializeField] private Transform smallGearSpawnLocation = null;

    [Header("Settings")]
    [SerializeField] private float transitionSpeed;

    private string interactText = "Interact";
    private bool lerping = false;
    private bool interacting = false;
    private bool puzzleComplete = false;
    private int correctChessPiecesCount = 0;
    private GameObject smallGearInstantiation = null;

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
        if (!interacting)
        {
            mouseLookScript.enabled = false;
            playerController.enabled = false;
            gameObject.layer = 0;
            lerping = true;
            interacting = true;
            interactingWithObject.Raise();
        }
    }

    public void OnEndHover()
    {
        onHoveringOverInteractableEnd.Raise();
    }

    public void OnStartHover()
    {
        onHoveringOverInteractable.Raise(GetInteractText());
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
        Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, chessBoardView.position, Time.deltaTime * transitionSpeed);

        Vector3 currentAngle = new Vector3(
           Mathf.LerpAngle(Camera.main.transform.rotation.eulerAngles.x, chessBoardView.transform.rotation.eulerAngles.x, Time.deltaTime * transitionSpeed),
           Mathf.LerpAngle(Camera.main.transform.rotation.eulerAngles.y, chessBoardView.transform.rotation.eulerAngles.y, Time.deltaTime * transitionSpeed),
           Mathf.LerpAngle(Camera.main.transform.rotation.eulerAngles.z, chessBoardView.transform.rotation.eulerAngles.z, Time.deltaTime * transitionSpeed));

        Camera.main.transform.eulerAngles = currentAngle;

        if (Vector3.Distance(Camera.main.transform.position, chessBoardView.transform.position) < 0.001f)
        {
            lerping = false;
            Camera.main.transform.position = chessBoardView.transform.position;
            Camera.main.transform.eulerAngles = chessBoardView.transform.eulerAngles;
            crosshair.enabled = false;
            Cursor.lockState = CursorLockMode.Confined;
        }
    }

    public void checkForCompletion()
    {
        correctChessPiecesCount++;
        if (correctChessPiecesCount == 5)
        {
            if (!puzzleComplete)
            {
                Debug.Log("Chess puzzle complete");
                smallGearInstantiation = Instantiate(smallGearPrefab, smallGearSpawnLocation.position, smallGearSpawnLocation.rotation);
                puzzleComplete = true;
                //play sound effect
                puzzleCompleted.Raise();
            }
        }
    }

    public void removeCorrectChessPiecesCount()
    {
        correctChessPiecesCount--;
        Debug.Log("A correct chess piece was removed");
    }

    [Serializable]
    private struct SaveData
    {
        public int correctChessPiecesCounter;
        public bool puzzleComplete;
        public bool gearHasBeenPickedUp;

    }

    public object CaptureState()
    {
        if ((smallGearInstantiation != null) && (smallGearInstantiation.GetComponent<Renderer>().enabled))
        {
            return new SaveData
            {
                correctChessPiecesCounter = correctChessPiecesCount,
                puzzleComplete = puzzleComplete,
                gearHasBeenPickedUp = false
            };
        }
        else
        {
            return new SaveData
            {
                correctChessPiecesCounter = correctChessPiecesCount,
                puzzleComplete = puzzleComplete,
                gearHasBeenPickedUp = true
            };
        }
    }

    public void RestoreState(object state)
    {
        var saveData = (SaveData)state;

        correctChessPiecesCount = saveData.correctChessPiecesCounter;
        puzzleComplete = saveData.puzzleComplete;
        if (puzzleComplete && !saveData.gearHasBeenPickedUp)
        {
            smallGearInstantiation = Instantiate(smallGearPrefab, smallGearSpawnLocation.position, smallGearSpawnLocation.rotation);
        }
    }
}
