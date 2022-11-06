using System;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;

public class AnimatedBookInteract : MonoBehaviour, IInteractable,ISaveable
{
    [SerializeField] protected StringEvent onHoveringOverInteractable = null;
    [SerializeField] protected VoidEvent onHoveringOverInteractableEnd = null;
    [SerializeField] private InputActionReference interactAction;
    [SerializeField] private AudioSource bookOpeningSound;
    [SerializeField] private AudioSource bookClosingSound;
    public float desiredDistanceFromCamera = 0.5f;

    private Animator bookAnimator = null;
    private bool bookOpen = false;
    private string interactText = "open the";
    private MeshCollider meshCollider = null;
    private BoxCollider boxCollider = null;


    public void Interact(GameObject other)
    {
        if (bookAnimator != null) //if the the game object has an animator
        {
            if (!bookOpen) //If the book is closed, Open
            {
                bookAnimator.Play("AnimatedBookOpening");
                bookOpeningSound.Play();
                bookOpen = true;
                interactText = "close the";
                meshCollider.enabled = false;
                boxCollider.enabled = true;
            }
            else  //If the book is open, close
            {
                bookAnimator.Play("AnimatedBookClosing");
                bookClosingSound.Play();
                bookOpen = false;
                interactText = "open the";
                meshCollider.enabled = true;
                boxCollider.enabled = false;
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
        bookAnimator = GetComponent<Animator>();
        meshCollider = GetComponent<MeshCollider>();
        boxCollider = GetComponent<BoxCollider>();
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
        public bool bookOpen;
    }

    public object CaptureState()
    {
        return new SaveData
        {
            bookOpen = bookOpen,
        };
    }

    public void RestoreState(object state)
    {
        var saveData = (SaveData)state;

        bookOpen = saveData.bookOpen;
        if (bookOpen)
        {
            GetComponent<Animator>().Play("AnimatedBookOpening"); ;
            interactText = "close the";
        }
    }
}
