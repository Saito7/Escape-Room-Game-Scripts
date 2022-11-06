using System;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;

public class DungeonDoorInteract : MonoBehaviour, IInteractable,ISaveable
{
    [SerializeField] protected StringEvent onHoveringOverInteractable = null;
    [SerializeField] protected VoidEvent onHoveringOverInteractableEnd = null;
    [SerializeField] private InputActionReference interactAction;
    [SerializeField] private AudioSource doorOpening;
    [SerializeField] private AudioSource doorClosing;
    [SerializeField] private AudioSource doorStillLocked;
    [SerializeField] private AudioSource doorUnlocking;

    private Animator doorAnimator = null;
    private bool doorOpen = false;
    private bool doorLocked = true;
    private string interactText = "open the";
    private string totalInteractText = null;

    public void Interact(GameObject other)
    {
        totalInteractText = GetInteractText();
        if (doorAnimator != null)
        {
            if (!doorOpen && !doorLocked)
            {
                doorAnimator.Play("DungeonDoorOpening");
                doorOpening.Play();
                doorOpen = true;
                interactText = "close the";
            }
            else if(!doorLocked && doorOpen)
            {
                doorAnimator.Play("DungeonDoorClosing");
                doorClosing.Play();
                doorOpen = false;
                interactText = "open the";
            }
            else
            {
                totalInteractText = "Locked";
                //play sound effect
                doorStillLocked.Play();
            }
            onHoveringOverInteractable.Raise(totalInteractText);
        }
        else
        {
            Debug.Log("Animator is missing");
        }
    }
    public void OnStartHover()
    {
        if (!doorLocked)
        {
            onHoveringOverInteractable.Raise(GetInteractText());
        }
        else
        {
            onHoveringOverInteractable.Raise("Locked");
        }
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

    public void doorUnlocked()
    {
        doorLocked = false;
        doorUnlocking.Play();        
    }

    [Serializable]
    private struct SaveData
    {
        public bool doorOpen;
        public bool doorLocked;
        public Vector3 doorPosition;
    }

    public object CaptureState()
    {
        return new SaveData
        {
            doorOpen = doorOpen,
            doorLocked = doorLocked,
        };
    }

    public void RestoreState(object state)
    {
        var saveData = (SaveData)state;

        doorOpen = saveData.doorOpen;
        doorLocked = saveData.doorLocked;
        if (doorOpen && !doorLocked)
        {
            GetComponent<Animator>().Play("DungeonDoorOpening");
            interactText = "close the";
        }
    }
}
