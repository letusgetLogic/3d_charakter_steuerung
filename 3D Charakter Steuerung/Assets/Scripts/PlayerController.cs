using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 150f;
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private float groundAcceleration = 1f;
    [SerializeField] private float airAcceleration = 0.7f;
    [SerializeField] private float gravityScale = 2f;
    
    [SerializeField] private Transform cameraPivotPointTransform;
    [SerializeField] private float lookSensitivityHorizontal = 10f; // Sensitivity of the camera movement horizontally.
    [SerializeField] private float lookSensitivityVertikal = 10f; // Sensitivity of the camera movement vertically.
    [SerializeField] private float lookDeepEndAngle = -50f; // Low end position of the inclination angle.
    [SerializeField] private float lookHighEndAngle = 80f; // High end position of the inclination angle.

    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.5f;
    
    [SerializeField] private float levelDominoPositionX = -72f;
    [SerializeField] private float levelDominoPositionY = 9f;
    [SerializeField] private float levelDominoPositionZ = 40f;

    private Rigidbody rb;
    private Transform playerTransform;

    private Vector2 moveInput;
    private Vector2 lookInput;

    //private Vector3 velocity;
    private Vector3 rotation; // Direction of horizontal movement of the camera.
    private Vector3 rotation2; // Direction of vertical movement of the camera.

    private void Awake()
    {
        rb = GetComponent<Rigidbody>(); 
        playerTransform = GetComponent<Transform>();
    }

    private void FixedUpdate()
    {
        /*OnDrawGizmos();*/
    }

    private void Update()
    {
        LookAround();

        Move3D();

        //rb.velocity = velocity;

        //Fall();
    }

    private void Move3D()
    {
        Vector3 moveDirection = Vector3.zero;


        float acceleration = isGrounded() ? groundAcceleration : airAcceleration;

        //velocity.x = Mathf.Lerp(velocity.x, moveInput.x * speed, acceleration * Time.deltaTime);
        //velocity.z = Mathf.Lerp(velocity.z, moveInput.z * speed, acceleration * Time.deltaTime);

        moveDirection.x = moveInput.x * speed * Time.deltaTime * acceleration; 

        moveDirection.y = rb.velocity.y;
        
        moveDirection.z = moveInput.y * speed * Time.deltaTime * acceleration;

        moveDirection = Quaternion.Euler(0, playerTransform.eulerAngles.y, 0) * moveDirection; // move direction
        rb.velocity = new Vector3(moveDirection.x, moveDirection.y, moveDirection.z);
    }

    private void LookAround()
    {
        //rotation.x -= lookInput.y * lookSensitivity * Time.deltaTime;

        rotation.y += lookInput.x * lookSensitivityHorizontal * Time.deltaTime; // Horizontal

        rotation2.x += lookInput.y * lookSensitivityVertikal * Time.deltaTime; // Vertical

        rotation2.x = Mathf.Clamp(rotation2.x, lookDeepEndAngle, lookHighEndAngle);
        rotation2.y = rotation.y;
        
        playerTransform.eulerAngles = rotation;

        cameraPivotPointTransform.eulerAngles = rotation2;
    }

    private void Fall()
    {
        if (rb.velocity.y < 0)
        {
            rb.velocity = new Vector3(0, rb.velocity.y * gravityScale * Time.deltaTime, 0);
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (isGrounded() && context.started == true)
        {
            rb.velocity = new Vector3(0, jumpForce, 0);
        }
    }

    ///
    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    /// <summary>
    /// Checks overlap of collider with groundChecks position on ground's collider.
    /// </summary>
    /// <returns></returns>
    bool isGrounded()
    {
        Collider[] colliders = Physics.OverlapSphere(groundCheck.position, groundCheckRadius, groundLayer);
        return colliders.Length > 0;
    }

    /*void OnDrawGizmos()
    {
        Gizmos.color = isGrounded() ? Color.green : Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }*/
}