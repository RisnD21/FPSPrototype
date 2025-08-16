using System.Collections.Generic;
using QuestDialogueSystem;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using System;

public class PlayerInput : MonoBehaviour
{
    [Header("Controls")]
    [SerializeField] private KeyCode forward = KeyCode.W;
    [SerializeField] private KeyCode back = KeyCode.S;
    [SerializeField] private KeyCode left = KeyCode.A;
    [SerializeField] private KeyCode right = KeyCode.D;
    [SerializeField] private KeyCode reload = KeyCode.R;
    [SerializeField] private KeyCode fire = KeyCode.Mouse0;
    [SerializeField] private KeyCode sprint = KeyCode.LeftShift;
    [SerializeField] private KeyCode interact = KeyCode.E;
    [SerializeField] private KeyCode inventory = KeyCode.Tab;

    [HideInInspector] public bool IsDialogueMode { get; private set; }
    

    [HideInInspector] public List<Button> options;

    public Vector2 InputVector => m_inputVector;
    private Vector2 m_inputVector;
    private float xInput;
    private float yInput;
    [HideInInspector] public bool IsReload;

    [HideInInspector] public bool IsFire;
    [HideInInspector] public bool IsSprint;
    [HideInInspector] public bool IsInteract;
    [HideInInspector] public int weaponIndex;

    public static event Action<MenuType> ToggleMenu;
    [HideInInspector] bool isMenuMode;

    void Awake()
    {
        options = new();
        weaponIndex = 0; //default
    }

    void OnEnable()
    {
        UIManager.SetMenuMode += SetMenuMode;
    }

    void OnDisable()
    {
        UIManager.SetMenuMode -= SetMenuMode;
    }


    public void HandleInput()
    {
        // Reset input
        xInput = 0;
        yInput = 0;

        if (Input.GetKey(forward))
        {
            yInput++;
        }

        if (Input.GetKey(back))
        {
            yInput--;
        }

        if (Input.GetKey(left))
        {
            xInput--;
        }

        if (Input.GetKey(right))
        {
            xInput++;
        }

        m_inputVector = new Vector2(xInput, yInput);

        if (Input.GetKeyDown(reload))
        {
            IsReload = true;
        }

        if (Input.GetKey(fire))
        {
            IsFire = true;
        }


        if (Input.GetKey(sprint))
        {
            IsSprint = true;
        }

        if (Input.GetKeyUp(sprint))
        {
            IsSprint = false;
        }

        if (Input.GetKeyDown(interact))
        {
            IsInteract = true;
        }


        for (int i = 0; i < 4; i++) //let's assume we can have atmost 4 weapons
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                weaponIndex = i;
                return;
            }
        } 
    }

    void PauseCharacterControl()
    {
        m_inputVector = Vector2.zero; //stop moving
    }

    public void HandleDialogueInput()
    {
        PauseCharacterControl();

        for (int i = 0; i < 9; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                if(i < options.Count )
                {
                    options[i].onClick.Invoke();
                    options.Clear();
                    return;
                }
            }
        } 

        if (Input.GetKeyDown(KeyCode.Escape)||Input.GetKey(forward)
        ||Input.GetKey(left)||Input.GetKey(right)||Input.GetKey(back))
            Locator.DialogueRunner.ResetDialog();
    }

    public void SetDialogMode(bool status)
    {
        IsDialogueMode = status;
        if(!IsDialogueMode) return;

        options.Clear();
    }

    void SetMenuMode(bool status)
    {
        isMenuMode = status;
    }


    private void Update()
    {
        if (Input.GetKeyDown(inventory))
        {
            ToggleMenu?.Invoke(MenuType.Inventory);
            if(isMenuMode) PauseCharacterControl();
        }

        if(isMenuMode) return;
        else if(IsDialogueMode) HandleDialogueInput();
        else HandleInput();
    }
}