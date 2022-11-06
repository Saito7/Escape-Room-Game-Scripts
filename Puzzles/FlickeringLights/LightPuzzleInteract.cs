using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;

public class LightPuzzleInteract : MonoBehaviour,IInteractable,ISaveable
{
    [Header("Events")]
    [SerializeField] protected StringEvent onHoveringOverInteractable = null;
    [SerializeField] protected VoidEvent onHoveringOverInteractableEnd = null;
    [SerializeField] protected IntEvent candlewasIgnited = null;
    [SerializeField] protected IntEvent candlewasExtinguished = null;
    [SerializeField] private InputActionReference interactAction;

    private HotbarItem playerHotbarSelected;
    private bool ignited = false;
    private string interactText = "Ignite";
    private string totalInteractText = null;

    public void Interact(GameObject other)
    {
        if (ignited)
        {
            interactText = "Ignite";
            candlewasExtinguished.Raise(transform.GetSiblingIndex());
            ignited = false;
            GetComponentInChildren<ToggleWithKeyPress>(true).SetActiveFalse();
            totalInteractText = GetInteractText();
        }
        else if(playerHotbarSelected != null)
        {
            if (playerHotbarSelected.Name == "Lighter")
            {
                interactText = "Extinguish";
                GetComponentInChildren<ToggleWithKeyPress>(true).SetActiveTrue();
                ignited = true;
                candlewasIgnited.Raise(transform.GetSiblingIndex());
                totalInteractText = GetInteractText();
            }
        }
        else
        {
            totalInteractText = "Lighter Required";
        }
        onHoveringOverInteractable.Raise(totalInteractText);
    }

    public void OnEndHover()
    {                            
        onHoveringOverInteractableEnd.Raise();
    }

    public void OnStartHover()
    {
        if (playerHotbarSelected != null && !ignited)
        {
            if (playerHotbarSelected.Name == "Lighter")
            {
                interactText = "Ignite";
                onHoveringOverInteractable.Raise(GetInteractText());
            }
        }
        else if (ignited)
        {
            interactText = "Extinguish";
            onHoveringOverInteractable.Raise(GetInteractText());
        }
        else
        {
            onHoveringOverInteractable.Raise("Lighter Required");
        }
    }

    private string GetInteractText()
    {
        int bindingIndex = interactAction.action.GetBindingIndexForControl(interactAction.action.controls[0]);

        StringBuilder builder = new StringBuilder();

        builder.Append("Press ").Append(InputControlPath.ToHumanReadableString(interactAction.action.bindings[bindingIndex].effectivePath,
            InputControlPath.HumanReadableStringOptions.OmitDevice)).Append(" to ").Append(interactText + " the " + gameObject.name);

        return builder.ToString();
    }

    public void Extinguish()
    {
        ignited = false;
        GetComponentInChildren<ToggleWithKeyPress>(true).SetActiveFalse();
    }

    public void getPlayerSelect(HotbarItem currentlySelected)
    {
        playerHotbarSelected = currentlySelected;
    }

    [Serializable]
    private struct SaveData
    {
        public bool isActive;
        public bool ignited;
        public string interactText;
    }

    public object CaptureState()
    {
        return new SaveData
        {
            isActive = GetComponentInChildren<ToggleWithKeyPress>(true).getActiveSelf(),
            ignited = ignited,
            interactText = interactText
        };
    }

    public void RestoreState(object state)
    {
        var saveData = (SaveData)state;

        if (saveData.isActive)
        {
            GetComponentInChildren<ToggleWithKeyPress>(true).SetActiveTrue();
        }
        else
        {
            GetComponentInChildren<ToggleWithKeyPress>(true).SetActiveFalse();
        }

        ignited = saveData.ignited;
        interactText = saveData.interactText;
    }
}
