using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

//控制所有面板的開關，並更新 PlayerInput 中的狀況
public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject inventoryUI;
    [SerializeField] GameObject inGameMenu;
    [SerializeField] GameObject crosshairUI;
    [SerializeField] GameObject gameOver;

    public static event Action<bool> SetMenuMode;
    Dictionary<GameObject, bool> menuStatus = new();

    void Start()
    {
        InitailzieUI();
    }

    void InitailzieUI()
    {
        menuStatus.Add(inventoryUI, true);
        menuStatus.Add(inGameMenu, true);
        menuStatus.Add(gameOver, true);
        inventoryUI.transform.localPosition = Vector3.zero;
        inGameMenu.transform.localPosition = Vector3.zero;


        var menus = new List<GameObject>(menuStatus.Keys);
        foreach (var menu in menus) ToggleMenu(menu);
    }

    void OnEnable()
    {
        PlayerInput.ToggleMenu += SelectMenu;
    }

    void OnDisable()
    {
        PlayerInput.ToggleMenu -= SelectMenu;
    }

    void SelectMenu(MenuType type)
    {
        GameObject menuToOpen = null;

        switch (type)
        {
            case MenuType.Inventory:
                menuToOpen = inventoryUI;
                break;
            case MenuType.InGameMenu:
                menuToOpen = inGameMenu;
                break;

            case MenuType.GameOver:
                menuToOpen = gameOver;
                Debug.Log("Player is killed");
                break;

            default:
                break;
        }

        ToggleMenu(menuToOpen);
    }

    void ToggleMenu(GameObject menu)
    {
        if (!menuStatus.ContainsKey(menu)) return;

        menuStatus[menu] = !menuStatus[menu];
        menu.SetActive(menuStatus[menu]);

        if (menuStatus.Any(kv => kv.Value))
        {
            SetMenuMode?.Invoke(true);
            crosshairUI.SetActive(false);
        }
        else
        {
            SetMenuMode?.Invoke(false);
            crosshairUI.SetActive(true);
        }
    }
}