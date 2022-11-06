using System;
using UnityEngine;

public enum ItemType { AnyItem, Book, Chess, Gear, PaintingDate, LogicGate, Firewood }

[Serializable]
public abstract class HotbarItem : ScriptableObject
{
    [Header("Basic Info")]
    public int ItemId = -1;
    [SerializeField] private new string name = "New Hotbar Item Name";
    [SerializeField] private Sprite icon = null;
    [SerializeField] private string infoText = null;
    [SerializeField] private GameObject prefab = null;
    [SerializeField] private ItemType itemType;

    //Getter functions

    public string Name => name; 

    public Sprite Icon => icon;

    public string InfoText => infoText;

    public GameObject Prefab => prefab;

    public ItemType ItemType => itemType;

    public abstract string GetInfoDisplayText();

    public abstract string GetInteractText();
}
