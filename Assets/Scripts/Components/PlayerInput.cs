using System.Collections.Generic;
using QuestDialogueSystem;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

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

    void Awake()
    {
        options = new();
        weaponIndex = 0; //default
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

    public void HandleDialogueInput()
    {
        m_inputVector = Vector2.zero; //stop moving

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

    private void Update()
    {
        if(IsDialogueMode) HandleDialogueInput();
        else HandleInput();
    }
}