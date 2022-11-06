using System;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SlidingTilePuzzleInteract : MonoBehaviour,IInteractable,ISaveable
{
    [Header("References")]
    [SerializeField] private InputActionReference interactAction;
    [SerializeField] private Transform tilePuzzleView = null;
    [SerializeField] private PlayerController playerController = null;
    [SerializeField] private MouseLook mouseLookScript = null;
    [SerializeField] private Image crosshair = null;
    [SerializeField] private SavingAndLoadingManager savingAndLoadingManager = null;
    [SerializeField] private GameObject gearPrefab = null;
    [SerializeField] private Transform largeGearSpawnLocation = null;

    [Header("Events")]
    [SerializeField] protected StringEvent onHoveringOverInteractable = null;
    [SerializeField] protected VoidEvent onHoveringOverInteractableEnd = null;
    [SerializeField] private VoidEvent interactingWithObject = null;
    [SerializeField] protected VoidEvent puzzleCompleted = null;

    [Header("Settings")]
    [SerializeField] private float transitionSpeed;

    private GameObject largeGearInstantiation = null;
    private bool puzzleComplete = false;
    private bool lerping = false;
    private bool interacting = false;
    private string interactText = "Interact";

    void Start()
    {
        if (!puzzleComplete)
        {
            string puzzleCompleteString = PlayerPrefs.GetString("TilePuzzle", string.Empty);
            if (string.IsNullOrEmpty(puzzleCompleteString)) { return; }
            if (puzzleCompleteString == "True")
            {
                Debug.Log("checking true");
                puzzleComplete = true;
                gameObject.layer = 0;
                puzzleCompleted.Raise();
                largeGearInstantiation = Instantiate(gearPrefab, largeGearSpawnLocation.position, largeGearSpawnLocation.rotation);
            }
        }
    }

    private void LateUpdate()
    {
        if (lerping)
        {
            MoveCameraAboveBoard();
        }
        if (!lerping && interacting)
        {
            savingAndLoadingManager.Save();
            Cursor.lockState = CursorLockMode.Confined;
            SceneManager.LoadScene(3);
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
        Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, tilePuzzleView.position, Time.deltaTime * transitionSpeed);

        Vector3 currentAngle = new Vector3(
           Mathf.LerpAngle(Camera.main.transform.rotation.eulerAngles.x, tilePuzzleView.transform.rotation.eulerAngles.x, Time.deltaTime * transitionSpeed),
           Mathf.LerpAngle(Camera.main.transform.rotation.eulerAngles.y, tilePuzzleView.transform.rotation.eulerAngles.y, Time.deltaTime * transitionSpeed),
           Mathf.LerpAngle(Camera.main.transform.rotation.eulerAngles.z, tilePuzzleView.transform.rotation.eulerAngles.z, Time.deltaTime * transitionSpeed));

        Camera.main.transform.eulerAngles = currentAngle;

        if (Vector3.Distance(Camera.main.transform.position, tilePuzzleView.transform.position) < 0.001f)
        {
            lerping = false;
            Camera.main.transform.position = tilePuzzleView.transform.position;
            Camera.main.transform.eulerAngles = tilePuzzleView.transform.eulerAngles;
            crosshair.enabled = false;
            Cursor.lockState = CursorLockMode.Confined;
        }
    }

    [Serializable]
    private struct SaveData
    {
        public bool puzzleComplete;
        public bool gearHasBeenPickedUp;
    }

    public object CaptureState()
    {
        if ((largeGearInstantiation != null) && (largeGearInstantiation.GetComponent<Renderer>().enabled))
        {
            return new SaveData
            {
                puzzleComplete = puzzleComplete,
                gearHasBeenPickedUp = false
            };
        }
        else
        {
            return new SaveData
            {
                puzzleComplete = puzzleComplete,
                gearHasBeenPickedUp = true
            };
        }
    }

    public void RestoreState(object state)
    {
        var saveData = (SaveData)state;

        puzzleComplete = saveData.puzzleComplete;
        if (puzzleComplete)
        {
            gameObject.layer = 0;
            if (!saveData.gearHasBeenPickedUp)
            {
                largeGearInstantiation = Instantiate(gearPrefab, largeGearSpawnLocation.position, largeGearSpawnLocation.rotation);
                Debug.Log("instantiating in state");
            }
        }
    }
}
