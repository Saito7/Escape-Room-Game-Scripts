using UnityEngine.InputSystem;
using UnityEngine;
using System.Text;
using UnityEngine.UI;

public class LogicGateInteract : MonoBehaviour, IInteractable
{
    [Header("Events")]
    [SerializeField] protected StringEvent onHoveringOverInteractable = null;
    [SerializeField] protected VoidEvent onHoveringOverInteractableEnd = null;
    [SerializeField] private VoidEvent interactingWithObject = null;
    [SerializeField] private VoidEvent endInteractingWithobject = null;

    [Header("References")]
    [SerializeField] private InputActionReference interactAction;
    [SerializeField] private Transform logicCameraView = null;
    [SerializeField] private PlayerController playerController = null;
    [SerializeField] private MouseLook mouseLookScript = null;
    [SerializeField] private Image crosshair = null;

    [Header("Settings")]
    [SerializeField] private float transitionSpeed;

    private string interactText = "Interact";
    private bool lerping = false;
    private bool interacting = false;

    private void LateUpdate()
    {
        if (lerping)
        {
            MoveCameraAboveCircuit();
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

    private void MoveCameraAboveCircuit()
    {
        //Lerp position of camera
        Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, logicCameraView.position, Time.deltaTime * transitionSpeed);

        Vector3 currentAngle = new Vector3(
           Mathf.LerpAngle(Camera.main.transform.rotation.eulerAngles.x, logicCameraView.transform.rotation.eulerAngles.x, Time.deltaTime * transitionSpeed),
           Mathf.LerpAngle(Camera.main.transform.rotation.eulerAngles.y, logicCameraView.transform.rotation.eulerAngles.y, Time.deltaTime * transitionSpeed),
           Mathf.LerpAngle(Camera.main.transform.rotation.eulerAngles.z, logicCameraView.transform.rotation.eulerAngles.z, Time.deltaTime * transitionSpeed));

        Camera.main.transform.eulerAngles = currentAngle;

        if (Vector3.Distance(Camera.main.transform.position, logicCameraView.transform.position) < 0.001f)
        {
            lerping = false;
            Camera.main.transform.position = logicCameraView.transform.position;
            Camera.main.transform.eulerAngles = logicCameraView.transform.eulerAngles;
            crosshair.enabled = false;
            Cursor.lockState = CursorLockMode.Confined;
        }
    }
}
