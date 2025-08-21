using System;
using QuestDialogueSystem;
using UnityEngine;

//define how different action should behave
//this is the early version of the state controller
//(compare to  AIAgent's NPC behavior script)
public class ActionStates : MonoBehaviour
{
    public enum ActionState
    {
        Idle,
        Moving,
        Firing,
        Reloading
    }

    Animator m_Animator;

    [HideInInspector] public Weapon currentWeapon;

    int fireNameHash = Animator.StringToHash("FireLoop");
    int reloadNameHash = Animator.StringToHash("Reloading");
    int moveNameHash = Animator.StringToHash("Move");
    
    ActionState m_CurrentAction;
    public ActionState CurrentAction => m_CurrentAction;

    void Awake()
    {
        currentWeapon = GetComponentInChildren<Weapon>(true); //use first weapon by default
    }

    void Start()
    {
        Select(currentWeapon);
    }

    public void Select(Weapon weapon)
    {
        if(weapon == null) return;
        if(currentWeapon != null) currentWeapon.gameObject.SetActive(false);

        weapon.gameObject.SetActive(true);
        currentWeapon = weapon;
        
        currentWeapon.LoadAmmo(GetComponent<IInventory>());
        m_Animator = currentWeapon.GetComponent<Animator>();
    }

    public void Fire()
    {
        if(CurrentAction == ActionState.Reloading) return;

        m_Animator.SetBool("Fire", true);
        currentWeapon.Fire();
    }

    public void StopFire()
    {
        if(CurrentAction == ActionState.Firing) m_Animator.SetBool("Fire", false);
    }  

    public void Reload()
    {
        if(CurrentAction == ActionState.Reloading || CurrentAction == ActionState.Firing) return;
        m_Animator.SetTrigger("Reload"); 
    }


    public void OnMove(bool isMoving)
    {
        if(CurrentAction == ActionState.Firing || CurrentAction == ActionState.Reloading) return;

        m_Animator.SetBool("isMoving",isMoving);
    }

    void Update()
    {
        UpdateControllerState();
    }

    void UpdateControllerState()
    {    
        if(m_Animator == null) return;

        var info = m_Animator.GetCurrentAnimatorStateInfo(0);

        ActionState newState;
        
        if (info.shortNameHash == fireNameHash)
        {
            newState = ActionState.Firing;
        }
        else if (info.shortNameHash == reloadNameHash)
        {
            newState = ActionState.Reloading;
        }
        else if (info.shortNameHash == moveNameHash)        
            newState = ActionState.Moving;
        else
        {
            newState = ActionState.Idle;
        }

        if (newState != m_CurrentAction)
        {
            var oldState = m_CurrentAction;
            m_CurrentAction = newState;
            
            if (currentWeapon.autoReload && oldState == ActionState.Firing)
            {
                if(currentWeapon.MustReload)
                    Reload();
                    
            } else if (oldState == ActionState.Reloading)
            {
                currentWeapon.Reload();   
            }
        }
    }
}
