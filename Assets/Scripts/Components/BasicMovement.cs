using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BasicMovement : MonoBehaviour
{
    [SerializeField] PlayerInput playerInput;
    [SerializeField] float walkSpeed = 9.0f;
    [SerializeField] float sprintSpeed = 16.0f;
    [SerializeField] float acceleration = 10.0f;
    [SerializeField] float tolerance = 0.1f;
    [SerializeField] float zoomSpeed = 5f;
    Rigidbody2D rb;
    CinemachineCamera cam;
    Vector2 targetSpeed;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        cam = GetComponent<CinemachineCamera>();
    }

    void Update()
    {
        Move();
    }
    void Move()
    {
        Vector2 inputVector = playerInput.InputVector;

        float maxSpeed = playerInput.IsSprint ? sprintSpeed : walkSpeed;

        targetSpeed = inputVector.magnitude < tolerance ? Vector2.zero : maxSpeed * inputVector;

        Vector2 currentSpeed = rb.linearVelocity;

        if ((currentSpeed - targetSpeed).magnitude < tolerance)
        {
            rb.linearVelocity = targetSpeed;
        }
        else
        {
            rb.linearVelocityX = Mathf.Lerp(currentSpeed.x, targetSpeed.x, Time.fixedDeltaTime * acceleration);
            rb.linearVelocityY = Mathf.Lerp(currentSpeed.y, targetSpeed.y, Time.fixedDeltaTime * acceleration);
        }
    }

    public void ZoomIn()
    {
        Debug.Log("Zoom In");
        cam.Lens.OrthographicSize -= Mathf.Max(0, 1 * Time.deltaTime) * zoomSpeed;
    }
    public void ZoomOut()
    {
        Debug.Log("Zoom Out");
        cam.Lens.OrthographicSize += Mathf.Max(0, 1 * Time.deltaTime) * zoomSpeed;
    }
}
