using System.Collections;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CharacterController))]
public class PlayerCharacter : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 5f;             // Walking movement speed
    //[SerializeField] private float runSpeed = 8f;              // Running speed
    [SerializeField] private float jumpForce = 4f;           // Force applied for jumping
    [SerializeField] private float groundCheckDistance = 1.2f;
    [SerializeField] private LayerMask groundMask;
    private bool isGrounded;                                   // Check whether player is grounded
    private Vector3 jumpingVelocity;

    [Header("Mouse Look Settings")]
    [SerializeField] private float lookSensitivity = 2f;       // Mouse sensitivity for smooth look
    [SerializeField] private float verticalLookLimit = 85f;    // Limit for looking up/down vertically
    private float verticalLookRotation;

    private CharacterController controller;
    private Rigidbody rb;                                      // Rigidbody for physics-based movement
    private Transform cameraTransform;                         // Reference to the player's camera
    private Animator animator;
    
    [SerializeField] private LayerMask enemyLayer;
    
    [SerializeField] private GameObject leftHandObject;
    
    
    //Melee Ability
    private bool hasMeleeAbility = true;
    private bool activeMelee;
    [SerializeField] private float meleeDamage = 70f;
    
    //Fireball Ability
    private bool hasFireballAbility = true;
    private bool activeFireball;
    private GameObject fireball;
    
    //Trioball Ability
    private bool hasTrioballAbility = true;
    private bool activeTrio;
    private GameObject trioball;
    
    //Pistol Ability
    private bool hasPistolAbility = true;

    void Start()
    {
        // References
        controller = GetComponent<CharacterController>();
        rb = GetComponent<Rigidbody>();
        cameraTransform = Camera.main.transform;
        animator = GetComponentInChildren<Animator>();

        fireball = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Fireball.prefab");
        trioball = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Trioball.prefab");

        // Lock cursor for FPS-style camera controls
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        CheckGrounded();
        HandleMovement();
        HandleMouseLook();
        HandleJump();
        
        if (Input.GetKeyDown(KeyCode.V) && hasMeleeAbility && !activeMelee)
        {
            StartCoroutine(MeleeAttack());
        }

        if (Input.GetKeyDown(KeyCode.Mouse1) && hasTrioballAbility && !activeTrio)
        {
            StartCoroutine(AltRangedAttack());
        }

        if (Input.GetKeyDown(KeyCode.Mouse0) && hasPistolAbility)
        {
            
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            hasMeleeAbility = !hasMeleeAbility;
        }
        
        if (Input.GetKeyDown(KeyCode.Q) && hasFireballAbility && !activeFireball)
        {
            StartCoroutine(Fireball());
        }
    }

    void HandleMovement()
    {
        // Capture input (Keyboard: WASD / Arrow Keys)
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        //bool isRunning = Input.GetKey(KeyCode.LeftShift);

        // Scale movement speed based on running or walking
        float currentSpeed = walkSpeed;  //(isRunning ? runSpeed : walkSpeed);
        
        // Calculate movement inputs relative to direction player is facing
        Vector3 move = transform.right * moveX * currentSpeed + transform.forward * moveZ * currentSpeed;

        controller.Move(move * Time.deltaTime);
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
    
    void CheckGrounded()
    {
        // Perform ground detection using a SphereCast
        isGrounded = Physics.CheckSphere(transform.position - Vector3.up * groundCheckDistance, 0.5f, groundMask);
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position - Vector3.up * groundCheckDistance, 0.5f);
    }

    void HandleJump()
    {
        // Listen for Jump input and ensure the player is grounded
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            jumpingVelocity.y = Mathf.Sqrt(jumpForce * -2f * Physics.gravity.y);
        }
        
        jumpingVelocity.y += Physics.gravity.y * Time.deltaTime;
        controller.Move(jumpingVelocity * Time.deltaTime);
    }

    private IEnumerator MeleeAttack()
    {
        animator.Play("Melee", 0, 0);

        activeMelee = true;
        
        yield return new WaitForSeconds(3f / 24f);
        
        RaycastHit[] hits = Physics.SphereCastAll(cameraTransform.position, 1f, Vector3.forward, 1f, enemyLayer);
        foreach (RaycastHit hit in hits)
        {
            hit.collider.TryGetComponent(out Health health);
            health.TakeDamage(meleeDamage);
        }
        
        yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(0).Length);
        activeMelee = false;
    }
    
    private IEnumerator AltRangedAttack()
    {
        animator.Play("Fireball", 0, 0);
        activeTrio = true;
        
        yield return new WaitForSeconds(9f / 24f);

        Instantiate(trioball, leftHandObject.transform.position, cameraTransform.rotation);
        
        yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(0).Length);
        activeTrio = false;
        
    }

    private IEnumerator Fireball()
    {
        animator.Play("Fireball", 0, 0);
        activeFireball = true;
        
        yield return new WaitForSeconds(9f / 24f);
        
        Instantiate(fireball, leftHandObject.transform.position, cameraTransform.rotation);
        
        yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(0).Length);
        activeFireball = false;
    }
}
