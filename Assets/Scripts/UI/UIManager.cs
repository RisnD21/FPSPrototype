using UnityEngine;
using System;

//控制所有面板的開關，並更新 PlayerInput 中的狀況
public class UIManager : MonoBehaviour
{
    [SerializeField] InventoryUI inventoryUI;
    [SerializeField] InGameMenu inGameMenu;

    public static event Action<bool> SetMenuMode;

    bool IsInventoryOpen;
    bool IsInGameMenuOpen;

    void Start()
    {
        InitailzieUI();
    }

    void InitailzieUI()
    {
        IsInventoryOpen = true;
        IsInGameMenuOpen = true;
        ToggleInventory();
    }

    void OnEnable()
    {
        PlayerInput.ToggleMenu += ToggleMenu;
    }

    void OnDisable()
    {
        PlayerInput.ToggleMenu -= ToggleMenu;
    }

    void ToggleMenu(MenuType type)
    {
        switch (type)
        {
            case MenuType.Inventory:
                ToggleInventory();
                break;

            default:
                break;
        }
    }

    void ToggleInventory()
    {
        IsInventoryOpen = !IsInventoryOpen;
        inventoryUI.gameObject.SetActive(IsInventoryOpen);

        if (!IsInventoryOpen) SetMenuMode?.Invoke(false);
        else SetMenuMode?.Invoke(true);
    }
    
    
}