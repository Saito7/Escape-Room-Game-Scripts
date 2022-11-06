using System;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;

public class VanityDeskDrawerInteract : MonoBehaviour, IInteractable,ISaveable
{
    [SerializeField] protected StringEvent onHoveringOverInteractable = null;
    [SerializeField] protected VoidEvent onHoveringOverInteractableEnd = null;
    [SerializeField] private InputActionReference interactAction;
    [SerializeField] private AudioSource drawerOpeningSound;
    [SerializeField] private AudioSource drawerClosingSound;

    private bool drawerOpen = false;
    private bool posValuesAlreadySet = false;
    private string interactText = "open the";
    private Vector3 currentPos = Vector3.zero;
    private Vector3 newPos = Vector3.zero;

    private void Start()
    {
        if (!posValuesAlreadySet)
        {
            currentPos = transform.localPosition;
            newPos = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z + 0.1948536f);
        }
    }

    public void Interact(GameObject other)
    {
        if (!drawerOpen)
        {
            transform.localPosition = Vector3.Lerp(currentPos, newPos, 1);
            drawerOpeningSound.Play();
            drawerOpen = true;
            interactText = "close the";
        }
        else
        {
            transform.localPosition = Vector3.Lerp(newPos, currentPos, 1);
            drawerClosingSound.Play();
            drawerOpen = false;
            interactText = "open the";
        }
        onHoveringOverInteractable.Raise(GetInteractText());
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
            InputControlPath.HumanReadableStringOptions.OmitDevice)).Append(" to ").Append(interactText + " " + gameObject.name);

        return builder.ToString();
    }

    [Serializable]
    private struct SaveData
    {
        public bool drawerOpen;
        public bool drawerLocked;
        public string interactText;
        public Vector3 endPos;
        public Vector3 startPos;
        public Vector3 position;
    }

    public object CaptureState()
    {
        return new SaveData
        {
            drawerOpen = drawerOpen,
            interactText = interactText,
            endPos = newPos,
            startPos = currentPos,
            position = transform.localPosition
        };
    }

    public void RestoreState(object state)
    {
        var saveData = (SaveData)state;

        drawerOpen = saveData.drawerOpen;
        interactText = saveData.interactText;
        transform.localPosition = saveData.position;
        currentPos = saveData.startPos;
        newPos = saveData.endPos;
        posValuesAlreadySet = true;
    }
}
