using System;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class WeighingScaleManager : MonoBehaviour,IInteractable,ISaveable
{
    [Header("Events")]
    [SerializeField] protected StringEvent onHoveringOverInteractable = null;
    [SerializeField] protected VoidEvent onHoveringOverInteractableEnd = null;
    [SerializeField] private VoidEvent interactingWithObject = null;
    [SerializeField] private VoidEvent endInteractingWithobject = null;
    [SerializeField] protected VoidEvent puzzleCompleted = null;

    [Header("References")]
    [SerializeField] private InputActionReference interactAction;
    [SerializeField] private Transform weighingScaleView = null;
    [SerializeField] private PlayerController playerController = null;
    [SerializeField] private MouseLook mouseLookScript = null;
    [SerializeField] private Image crosshair = null;
    [SerializeField] private GameObject firewoodPrefab = null;
    [SerializeField] private Transform firewoodSpawnLocation = null;

    [Header("Settings")]
    [SerializeField] private float transitionSpeed;

    private GameObject firewoodInstantiation = null;
    private string interactText = "Interact";
    private bool lerping = false;
    private bool interacting = false;
    private bool puzzleComplete = false;
    private int itemOneWeight = -1;
    private int itemTwoWeight = -2;

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
        Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, weighingScaleView.position, Time.deltaTime * transitionSpeed);

        Vector3 currentAngle = new Vector3(
           Mathf.LerpAngle(Camera.main.transform.rotation.eulerAngles.x, weighingScaleView.transform.rotation.eulerAngles.x, Time.deltaTime * transitionSpeed),
           Mathf.LerpAngle(Camera.main.transform.rotation.eulerAngles.y, weighingScaleView.transform.rotation.eulerAngles.y, Time.deltaTime * transitionSpeed),
           Mathf.LerpAngle(Camera.main.transform.rotation.eulerAngles.z, weighingScaleView.transform.rotation.eulerAngles.z, Time.deltaTime * transitionSpeed));

        Camera.main.transform.eulerAngles = currentAngle;

        if (Vector3.Distance(Camera.main.transform.position, weighingScaleView.transform.position) < 0.001f)
        {
            lerping = false;
            Camera.main.transform.position = weighingScaleView.transform.position;
            Camera.main.transform.eulerAngles = weighingScaleView.transform.eulerAngles;
            crosshair.enabled = false;
            Cursor.lockState = CursorLockMode.Confined;
        }
    }

    public void checkForCompletion()
    {
        if (itemOneWeight != 0 && itemTwoWeight != 0)
        {
            if (itemOneWeight == itemTwoWeight)
            {
                if (!puzzleComplete)
                {
                    Debug.Log("Weighing Scale puzzle completed");
                    firewoodInstantiation =  Instantiate(firewoodPrefab, firewoodSpawnLocation.position, firewoodSpawnLocation.rotation);
                    puzzleComplete = true;
                    //play completed noise
                    puzzleCompleted.Raise();
                }
            }
        }
    }

    public void setItemWeight(int weight, int index)
    {
        if (index == 0)
        {
            itemOneWeight = weight;
        }
        if(index == 1)
        {
            itemTwoWeight = weight;
        }
        checkForCompletion();
    }

    [Serializable]
    private struct SaveData
    {
        public bool puzzleComplete;
        public bool firewoodHasBeenPickedUp;

    }

    public object CaptureState()
    {
        if ((firewoodInstantiation != null) && (firewoodInstantiation.GetComponent<Renderer>().enabled))
        {
            return new SaveData
            {
                puzzleComplete = puzzleComplete,
                firewoodHasBeenPickedUp = false
            };
        }
        else
        {
            return new SaveData
            {
                puzzleComplete = puzzleComplete,
                firewoodHasBeenPickedUp = true
            };
        }
    }

    public void RestoreState(object state)
    {
        var saveData = (SaveData)state;

        puzzleComplete = saveData.puzzleComplete;
        if (puzzleComplete && !saveData.firewoodHasBeenPickedUp)
        {
            firewoodInstantiation = Instantiate(firewoodPrefab, firewoodSpawnLocation.position, firewoodSpawnLocation.rotation);
        }
    }
}
