using UnityEngine;
using UnityEngine.InputSystem;

public class HotbarInputManager : MonoBehaviour
{
    [SerializeField] private Hotbar hotbarInput = null;
    private HotbarItem availableItemToQuickAdd = null;

    // Update is called once per frame
    void Update()
    {
        checkForQuickAdd();
        wasHotbarItemSelected();
    }

    private void wasHotbarItemSelected()
    {
        if (Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            hotbarInput.Use(1);
        }
        if (Keyboard.current.digit2Key.wasPressedThisFrame)
        {
            hotbarInput.Use(2);
        }
        if (Keyboard.current.digit3Key.wasPressedThisFrame)
        {
            hotbarInput.Use(3);
        }
        if (Keyboard.current.digit4Key.wasPressedThisFrame)
        {
            hotbarInput.Use(4);
        }
        if (Keyboard.current.digit5Key.wasPressedThisFrame)
        {
            hotbarInput.Use(5);
        }
        if (Keyboard.current.digit6Key.wasPressedThisFrame)
        {
            hotbarInput.Use(6);
        }
        if (Keyboard.current.digit7Key.wasPressedThisFrame)
        {
            hotbarInput.Use(7);
        }
        if (Keyboard.current.digit8Key.wasPressedThisFrame)
        {
            hotbarInput.Use(8);
        }
        if (Keyboard.current.digit9Key.wasPressedThisFrame)
        {
            hotbarInput.Use(9);
        }
        if (Keyboard.current.digit0Key.wasPressedThisFrame)
        {
            hotbarInput.Use(10);
        }
    }

    private void checkForQuickAdd()
    {
        if(availableItemToQuickAdd != null)
        {
            if(Mouse.current.leftButton.wasPressedThisFrame && Keyboard.current.leftShiftKey.isPressed)
            {
                hotbarInput.Add(availableItemToQuickAdd);
            }
        }
    }

    public void quickAdd(HotbarItem hotbarItem)
    {
        availableItemToQuickAdd = hotbarItem;
    }

    public void quickRemove()
    {
        availableItemToQuickAdd = null;
    }
}
