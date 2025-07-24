using Unity.VisualScripting;
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

    Weapon currentWeapon;

    int fireNameHash = Animator.StringToHash("Firing");
    int reloadNameHash = Animator.StringToHash("Reloading");
    int moveNameHash = Animator.StringToHash("Move");
    
    ActionState m_CurrentAction;
    public ActionState CurrentAction => m_CurrentAction;

    void Awake()
    {
        Initialize();
    }

    public void Select(Weapon weapon)
    {
        currentWeapon = weapon;
    }

    public void Initialize()
    {
        //add animator list later on, with corresponding setting
        m_Animator = GetComponentInChildren<Animator>();
    }

    public void Fire()
    {
        if(CurrentAction == ActionState.Firing || CurrentAction == ActionState.Reloading) return;

        m_Animator.SetTrigger("Fire");

        currentWeapon.Fire();
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
        var info = m_Animator.GetCurrentAnimatorStateInfo(0);

        ActionState newState;
        
        if (info.shortNameHash == fireNameHash)
            newState = ActionState.Firing;
        else if (info.shortNameHash == reloadNameHash)
            newState = ActionState.Reloading;
        else if (info.shortNameHash == moveNameHash)        
            newState = ActionState.Moving;
        else
            newState = ActionState.Idle;

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
