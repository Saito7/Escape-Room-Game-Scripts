using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class KeyBinding : MonoBehaviour
{
    [SerializeField] private InputActionReference actionToChange = null;
    [SerializeField] private InputActionAsset playerInput = null;
    [SerializeField] private TMP_Text bindingDisplayNameText = null;
    [SerializeField] private GameObject startRebindObject = null;
    [SerializeField] private GameObject waitingForInputObject = null;

    private InputActionRebindingExtensions.RebindingOperation rebindingOperation;

    private const string RebindsKey = "rebinds";

    private void Start()
    {
        //Load keybinds on game start
        string rebinds = PlayerPrefs.GetString(RebindsKey, string.Empty);

        if (string.IsNullOrEmpty(rebinds)) { return; }

        playerInput.LoadBindingOverridesFromJson(rebinds); //load key bindings

        int bindingIndex = actionToChange.action.GetBindingIndexForControl(actionToChange.action.controls[0]);

        bindingDisplayNameText.text = InputControlPath.ToHumanReadableString(actionToChange.action.bindings[bindingIndex].effectivePath,
            InputControlPath.HumanReadableStringOptions.OmitDevice);  //Input system will take this and convert it into a human readable string ignoring keyboard/controller etc
    }

    public void StartRebinding()
    {
        startRebindObject.SetActive(false); //turn button off
        waitingForInputObject.SetActive(true); //display waiting for input


        rebindingOperation = actionToChange.action.PerformInteractiveRebinding()
            .WithControlsExcluding("Mouse")         //ignore mouse input
            .WithCancelingThrough("Escape")
            .OnMatchWaitForAnother(0.1f)            //wait for 0.1s
            .OnComplete(operation => RebindComplete())
            .Start();               //when action performed, call new method
    }

    private void RebindComplete()
    {
        int bindingIndex = actionToChange.action.GetBindingIndexForControl(actionToChange.action.controls[0]);

        bindingDisplayNameText.text = InputControlPath.ToHumanReadableString(actionToChange.action.bindings[bindingIndex].effectivePath,
            InputControlPath.HumanReadableStringOptions.OmitDevice);  //Input system will take this and convert it into a human readable string ignoring keyboard/controller etc

        rebindingOperation.Dispose(); //to save memory

        startRebindObject.SetActive(true);
        waitingForInputObject.SetActive(false);
    }
}
