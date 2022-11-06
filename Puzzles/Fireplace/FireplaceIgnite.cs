using System;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class FireplaceIgnite : MonoBehaviour, IInteractable, ISaveable
{
    [Header("Events")]
    [SerializeField] protected StringEvent onHoveringOverInteractable = null;
    [SerializeField] protected VoidEvent onHoveringOverInteractableEnd = null;
    [SerializeField] protected VoidEvent puzzleCompleted = null;

    [Header("References")]
    [SerializeField] private InputActionReference interactAction;


    private HotbarItem playerHotbarSelected = null;
    private string totalInteractText = null;
    private string interactText = "Ignite";
    private bool puzzleComplete = false;
    private bool fireplacePuzzleComplete = false;


    public void Interact(GameObject other)
    {
        totalInteractText = GetInteractText();

        if (playerHotbarSelected != null)
        {
            if (playerHotbarSelected.Name == "Lighter" && fireplacePuzzleComplete && !puzzleComplete)
            {
                interactText = "Ignite";
                GetComponentInChildren<ToggleWithKeyPress>(true).SetActiveTrue();
                puzzleComplete = true;
                puzzleCompleted.Raise();
                Invoke("gameComplete", 3f);
            }
        }
        else
        {
            totalInteractText = "Lighter Required";
        }
        onHoveringOverInteractable.Raise(totalInteractText);
    }

    public void gameComplete()
    {
        SceneManager.LoadScene(4);
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
            InputControlPath.HumanReadableStringOptions.OmitDevice)).Append(" to ").Append(interactText + " " + gameObject.name);

        return builder.ToString();
    }

    public void fireplacePuzzleCompleted()
    {
        fireplacePuzzleComplete = true;
    }

    public void getPlayerSelect(HotbarItem currentlySelected)
    {
        playerHotbarSelected = currentlySelected;
    }

    [Serializable]
    private struct SaveData
    {
        public bool fireplacePuzzleComplete;
        public bool puzzleComplete;
    }

    public object CaptureState()
    {
        return new SaveData
        {
            fireplacePuzzleComplete = fireplacePuzzleComplete,
            puzzleComplete = puzzleComplete
        };
    }

    public void RestoreState(object state)
    {
        var saveData = (SaveData)state;

        fireplacePuzzleComplete = saveData.fireplacePuzzleComplete;
        puzzleComplete = saveData.puzzleComplete;
        if (puzzleComplete)
        {
            gameComplete();
        }
    }
}
