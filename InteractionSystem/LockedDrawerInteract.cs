using System;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;

public class LockedDrawerInteract : MonoBehaviour, IInteractable,ISaveable
{
    [SerializeField] protected StringEvent onHoveringOverInteractable = null;
    [SerializeField] protected VoidEvent onHoveringOverInteractableEnd = null;
    [SerializeField] private InputActionReference interactAction;
    [SerializeField] private AudioSource drawerOpeningSound;
    [SerializeField] private AudioSource drawerClosingSound;
    [SerializeField] private AudioSource drawerLockedSound;
    [SerializeField] private AudioSource unlockingSound;

    private bool drawerOpen = false;
    private bool drawerLocked = true;
    private bool posValuesAlreadySet = false;
    private string interactText = "open the";
    private string totalInteractText = null;
    private Vector3 currentPos = Vector3.zero;
    private Vector3 newPos = Vector3.zero;

    private void Start()
    {
        if (!posValuesAlreadySet)
        {
            currentPos = transform.localPosition;
            newPos = new Vector3(transform.localPosition.x - 0.21f, transform.localPosition.y, transform.localPosition.z);
        }
    }

    public void Interact(GameObject other)
    {
        totalInteractText = GetInteractText();
        
        if (!drawerOpen && !drawerLocked)
        {
            transform.localPosition = Vector3.Lerp(currentPos, newPos, 1);
            drawerOpen = true;
            drawerOpeningSound.Play();
            interactText = "close the";
        }
        else if (!drawerLocked && drawerOpen)
        {
            transform.localPosition = Vector3.Lerp(newPos, currentPos, 1);
            drawerOpen = false;
            drawerClosingSound.Play();
            interactText = "open the";
        }
        else
        {
            drawerLockedSound.Play();
            totalInteractText = "Locked";
            //play sound effect
        }
        onHoveringOverInteractable.Raise(totalInteractText);
    }
    public void OnStartHover()
    {
        if (!drawerLocked)
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

    private string GetInteractText()
    {
        int bindingIndex = interactAction.action.GetBindingIndexForControl(interactAction.action.controls[0]);

        StringBuilder builder = new StringBuilder();

        builder.Append("Press ").Append(InputControlPath.ToHumanReadableString(interactAction.action.bindings[bindingIndex].effectivePath,
            InputControlPath.HumanReadableStringOptions.OmitDevice)).Append(" to ").Append(interactText + " " + gameObject.name);

        return builder.ToString();
    }

    public void drawerUnlocked()
    {
        drawerLocked = false;
        unlockingSound.Play();
    }

    [Serializable]
    private struct SaveData
    {
        public bool drawerOpen;
        public bool drawerLocked;
        public string interactText;
        public Vector3 position;
        public Vector3 startPos;
        public Vector3 endPos;
    }

    public object CaptureState()
    {
        return new SaveData
        {
            drawerOpen = drawerOpen,
            drawerLocked = drawerLocked,
            interactText = interactText,
            position = transform.localPosition,
            startPos = currentPos,
            endPos = newPos
        };
    }

    public void RestoreState(object state)
    {
        var saveData = (SaveData)state;

        drawerOpen = saveData.drawerOpen;
        drawerLocked = saveData.drawerLocked;
        interactText = saveData.interactText;
        transform.localPosition = saveData.position;
        currentPos = saveData.startPos;
        newPos = saveData.endPos;
        posValuesAlreadySet = true;
    }
}
