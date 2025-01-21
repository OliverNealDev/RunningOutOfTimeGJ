using UnityEditor;
using UnityEngine;

public enum RightHandAbility
{
    Gun,
    Fireball,
}

[RequireComponent(typeof(Rigidbody))]
public class PlayerCharacter : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;         // Walking movement speed
    public float runSpeed = 8f;         // Running speed
    public float acceleration = 10f;    // Movement acceleration
    public float deceleration = 10f;    // Movement deceleration
    public float jumpForce = 50f;        // Force applied for jumping

    [Header("Physics Settings")]
    public float gravityMultiplier = 9.81f; // Multiplier for stronger gravity
    public float groundCheckDistance = 0.9f; // Distance to check if grounded
    public LayerMask groundMask;         // Layers considered as "Ground"

    [Header("Mouse Look Settings")]
    public float lookSensitivity = 2f;  // Mouse sensitivity for smooth look
    public float verticalLookLimit = 85f; // Limit for looking up/down vertically

    private Rigidbody rb;                // Rigidbody for physics-based movement
    private Transform cameraTransform;   // Reference to the player's camera
    private Vector3 desiredVelocity;     // Target velocity based on input
    private bool isGrounded;             // Check whether player is grounded
    private float originalDrag;          // Store Rigidbody's default drag value
    private float verticalLookRotation;  // Tracks current vertical view angle
    
    [SerializeField] private LayerMask enemyLayer;
    
    //Melee Ability
    private bool hasMeleeAbility = true;
    [SerializeField] private float meleeDamage = 70f;
    
    //Fireball Ability
    private bool hasFireballAbility = true;
    private GameObject fireball;
    
    //Trioball Ability
    private bool hasTrioballAbility = true;
    private GameObject trioball;

    void Start()
    {
        // References
        rb = GetComponent<Rigidbody>();
        cameraTransform = Camera.main.transform;

        fireball = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Fireball.prefab");
        trioball = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Trioball.prefab");

        // Lock cursor for FPS-style camera controls
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Maintain drag for smooth deceleration
        originalDrag = rb.linearDamping;

        // Better gravity simulation
        rb.useGravity = false;
    }

    void Update()
    {
        HandleMouseLook();
        HandleJump();
        CheckGrounded();
        
        if (Input.GetKeyDown(KeyCode.V) && hasMeleeAbility)
        {
            MeleeAttack();
        }

        if (Input.GetKeyDown(KeyCode.Mouse0) && hasFireballAbility)
        {
            RangedAttack();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            hasMeleeAbility = !hasMeleeAbility;
        }
        
        if (Input.GetKeyDown(KeyCode.Q) && hasFireballAbility)
        {
            Fireball();
        }
    }

    void FixedUpdate()
    {
        HandleMovement();
        ApplyAdditionalGravity();
        
    }

    void HandleMovement()
    {
        // Capture input (Keyboard: WASD / Arrow Keys)
        float inputHorizontal = Input.GetAxis("Horizontal");
        float inputVertical = Input.GetAxis("Vertical");
        bool isRunning = Input.GetKey(KeyCode.LeftShift);

        // Calculate movement inputs relative to direction player is facing
        Vector3 moveDirection = transform.right * inputHorizontal + transform.forward * inputVertical;

        // Scale movement speed based on running or walking
        float targetSpeed = (isRunning ? runSpeed : walkSpeed);

        // Gradually adjust velocity using acceleration
        desiredVelocity = moveDirection.normalized * targetSpeed;
        rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, new Vector3(desiredVelocity.x, rb.linearVelocity.y, desiredVelocity.z), Time.fixedDeltaTime * (isGrounded ? acceleration : deceleration));
    }

    void HandleMouseLook()
    {
        // Get mouse input
        float mouseX = Input.GetAxis("Mouse X") * lookSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * lookSensitivity;

        // Rotate player horizontally (yaw rotation)
        transform.Rotate(Vector3.up * mouseX);

        // Rotate camera vertically (pitch rotation) and clamp angle
        verticalLookRotation -= mouseY;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -verticalLookLimit, verticalLookLimit);
        cameraTransform.localRotation = Quaternion.Euler(verticalLookRotation, 0f, 0f);
    }

    void HandleJump()
    {
        // Listen for Jump input and ensure the player is grounded
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z); // Reset Y velocity to prevent double jumps
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void ApplyAdditionalGravity()
    {
        // Apply stronger gravity manually to achieve a better feel
        if (!isGrounded)
        {
            rb.AddForce(Vector3.down * gravityMultiplier, ForceMode.Acceleration);
        }
    }

    void CheckGrounded()
    {
        // Perform ground detection using a SphereCast
        isGrounded = Physics.CheckSphere(transform.position - Vector3.up * groundCheckDistance, 0.5f, groundMask);

        // Modify drag to simulate sticky landing or sliding off
        rb.linearDamping = isGrounded ? originalDrag : 0f;
    }

    private void MeleeAttack()
    {
        RaycastHit[] hits = Physics.SphereCastAll(cameraTransform.position, 1f, Vector3.forward, 1f, enemyLayer);
        foreach (RaycastHit hit in hits)
        {
            hit.collider.TryGetComponent(out Health health);
            health.TakeDamage(meleeDamage);
        }
    }
    
    private void RangedAttack()
    {
        Instantiate(trioball, cameraTransform.position, cameraTransform.rotation);
    }

    private void Fireball()
    {
        Instantiate(fireball, cameraTransform.position, cameraTransform.rotation);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position - Vector3.up * groundCheckDistance, 0.5f);
    }
}
