using System;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;

public class DarkCabinetGlassDoorInteract1 : MonoBehaviour, IInteractable,ISaveable
{
    [SerializeField] protected StringEvent onHoveringOverInteractable = null;
    [SerializeField] protected VoidEvent onHoveringOverInteractableEnd = null;
    [SerializeField] private InputActionReference interactAction;
    [SerializeField] private AudioSource cabinetOpeningSound;
    [SerializeField] private AudioSource cabinetClosingSound;

    private Animator doorAnimator = null;
    private bool doorOpen = false;
    private string interactText = "open the";

    public void Interact(GameObject other)
    {
        if (doorAnimator != null)
        {
            if (!doorOpen)
            {
                doorAnimator.Play("GlassDoorOpening1");
                cabinetOpeningSound.Play();
                doorOpen = true;
                interactText = "close the";
            }
            else
            {
                doorAnimator.Play("GlassDoorClosing1");
                cabinetClosingSound.Play();
                doorOpen = false;
                interactText = "open the";
            }
            onHoveringOverInteractable.Raise(GetInteractText());
        }
        else
        {
            Debug.Log("Animator is missing");
        }
    }
    public void OnStartHover()
    {
        onHoveringOverInteractable.Raise(GetInteractText());
    }

    public void OnEndHover()
    {
        onHoveringOverInteractableEnd.Raise();
    }

    private void Start()
    {
        doorAnimator = GetComponent<Animator>();
    }

    private string GetInteractText()
    {
        int bindingIndex = interactAction.action.GetBindingIndexForControl(interactAction.action.controls[0]);

        StringBuilder builder = new StringBuilder();

        builder.Append("Press ").Append(InputControlPath.ToHumanReadableString(interactAction.action.bindings[bindingIndex].effectivePath,
            InputControlPath.HumanReadableStringOptions.OmitDevice)).Append(" to ").Append(interactText + " " + gameObject.name);

        return builder.ToString();
    }

    [Serializable]
    private struct SaveData
    {
        public bool doorOpen;
    }

    public object CaptureState()
    {
        return new SaveData
        {
            doorOpen = doorOpen,
        };
    }

    public void RestoreState(object state)
    {
        var saveData = (SaveData)state;

        doorOpen = saveData.doorOpen;
        if (doorOpen)
        {
            GetComponent<Animator>().Play("GlassDoorOpening1");
            interactText = "close the";
        }
    }
}
