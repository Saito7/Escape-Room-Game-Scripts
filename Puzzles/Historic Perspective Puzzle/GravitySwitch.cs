using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class GravitySwitch : MonoBehaviour, IInteractable
{
    [SerializeField] protected StringEvent onHoveringOverInteractable = null;
    [SerializeField] protected VoidEvent onHoveringOverInteractableEnd = null;
    [SerializeField] private InputActionReference interactAction;
    [SerializeField] private Transform target;
    public float gravityValue = -20f;
    public float shiftMultiplier = 0.2f;

    private Transform orientation;
    private string interactText = "Flip Gravity";

    public void Interact(GameObject other)
    {
        other.transform.rotation = Quaternion.LookRotation(target.forward, target.up);
        orientation = other.GetComponent<GravityPlayerController>().orientation;
        other.transform.position += orientation.forward * shiftMultiplier;
        Physics.gravity = orientation.up.normalized * gravityValue;
    }
    public void OnStartHover()
    {
        onHoveringOverInteractable.Raise(GetInteractText());
    }

    public void OnEndHover()
    {
        onHoveringOverInteractableEnd.Raise();
    }


    private string GetInteractText()
    {
        int bindingIndex = interactAction.action.GetBindingIndexForControl(interactAction.action.controls[0]);

        StringBuilder builder = new StringBuilder();

        builder.Append("Press ").Append(InputControlPath.ToHumanReadableString(interactAction.action.bindings[bindingIndex].effectivePath,
            InputControlPath.HumanReadableStringOptions.OmitDevice)).Append(" to ").Append(interactText);

        return builder.ToString();
    }

}
