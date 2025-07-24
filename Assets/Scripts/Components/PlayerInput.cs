using Unity.VisualScripting;
using UnityEngine;

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

    public Vector2 InputVector => m_inputVector;
    private Vector2 m_inputVector;
    private float xInput;
    private float yInput;
    public bool IsReload;

    public bool IsFire;
    public bool IsSprint;

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

        if (Input.GetKeyDown(fire))
        {
            IsFire = true;
        }

        if (Input.GetKeyDown(sprint))
        {
            IsSprint = true;
        }

        if (Input.GetKeyUp(sprint))
        {
            IsSprint = false;
        }
    }

    private void Update()
    {
        HandleInput();
    }
}