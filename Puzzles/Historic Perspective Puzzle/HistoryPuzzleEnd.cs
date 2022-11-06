using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class HistoryPuzzleEnd : MonoBehaviour, IInteractable
{
    [SerializeField] protected StringEvent onHoveringOverInteractable = null;
    [SerializeField] protected VoidEvent onHoveringOverInteractableEnd = null;
    [SerializeField] private VoidEvent puzzleComplete;
    [SerializeField] private InputActionReference interactAction;
    [SerializeField] private LevelLoader levelLoader;

    private string interactText = "Store";

    public void Interact(GameObject other)
    {
        puzzleComplete.Raise();
        PlayerPrefs.SetInt("Historic", 1);
        PlayerPrefs.SetInt("currentScene", 2);
        levelLoader.LoadNextLevel(2);   
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
            InputControlPath.HumanReadableStringOptions.OmitDevice)).Append(" to ").Append(interactText + " Firewood");

        return builder.ToString();
    }
}
