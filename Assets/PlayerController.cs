using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public enum AnimationLayer
{
    Left = 0,
    Right = 1,
}

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 5f;             // Walking movement speed
    //[SerializeField] private float runSpeed = 8f;              // Running speed
    [SerializeField] private float jumpForce = 4f;           // Force applied for jumping
    [SerializeField] private float groundCheckDistance = 1.2f;
    [FormerlySerializedAs("groundMask")] [SerializeField] private LayerMask groundLayer;
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
    private Health health;
    
    [SerializeField] private LayerMask enemyLayer;
    
    [SerializeField] private GameObject leftHandObject;
    
    //Melee Ability
    private bool hasMeleeAbility = true;
    private bool activeMelee;
    private float meleeCooldown = 2f;
    private float meleeDamage = 220f;
    [SerializeField] private AnimationClip meleeAnimation;
    [SerializeField] private AnimationLayer meleeAnimationLayer;

    private bool activeAbility;
    
    //Fireball Ability
    private bool hasFireballAbility = true;
    private bool activeFireball;
    private float fireballCooldown = 7f;
    [SerializeField] private GameObject fireball;
    [SerializeField] private AnimationClip fireballAnimation;
    [SerializeField] private AnimationLayer fireballAnimationLayer;
    
    //Trioball Ability
    private bool hasTrioballAbility = true;
    private bool activeTrio;
    private float trioballCooldown = 3f;
    [SerializeField] private GameObject trioball;
    
    //Pistol Ability
    private bool hasPistolAbility = true;
    private bool activePistol;
    private float pistolDamage = 85f;
    [SerializeField] private AnimationClip shootAnimation;
    [SerializeField] private AnimationLayer shootAnimationLayer;
    [SerializeField] private AnimationClip leftReloadAnimation;
    [SerializeField] private AnimationClip rightReloadAnimation;

    //Dash Ability
    private bool hasDashAbility = true;
    private bool activeDash;
    private float dashCooldown = 2.5f;
    private float dashForce = 20f;
    private float dashDuration = 0.25f;
    private Vector3 dashVelocity;

    void Start()
    {
        // References
        controller = GetComponent<CharacterController>();
        rb = GetComponent<Rigidbody>();
        cameraTransform = Camera.main.transform;
        animator = GetComponentInChildren<Animator>();
        health = GetComponent<Health>();

        // Lock cursor for FPS-style camera controls
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (Time.timeScale == 0f) return;
        
        CheckGrounded();
        HandleMovement();
        HandleMouseLook();
        HandleJump();
        
        if (Input.GetKeyDown(KeyCode.E) && hasMeleeAbility && !activeMelee && !activeAbility)
        {
            StartCoroutine(MeleeAttack());
        }

        if (Input.GetKeyDown(KeyCode.Mouse1) && hasTrioballAbility && !activeTrio && !activeAbility)
        {
            StartCoroutine(AltRangedAttack());
        }

        if (Input.GetKey(KeyCode.Mouse0) && hasPistolAbility && !activePistol)
        {
            StartCoroutine(PistolAttack());
        }
        
        if (Input.GetKeyDown(KeyCode.Q) && hasFireballAbility && !activeFireball && !activeAbility)
        {
            StartCoroutine(Fireball());
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && hasDashAbility && !activeDash)
        {
            StartCoroutine(Dash());
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
        isGrounded = Physics.CheckSphere(transform.position - Vector3.up * groundCheckDistance, 0.5f, groundLayer);
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

    public void ToggleAbility(bool active, int index)
    {
        switch (index)
        {
            case 1:
                hasMeleeAbility = active;
                break;
            case 2:
                hasFireballAbility = active;
                break;
            case 3:
                hasDashAbility = active;
                break;
            case 4:
                hasPistolAbility = active;
                break;
            case 5: 
                hasTrioballAbility = active;
                break;
        }
    }

    private IEnumerator MeleeAttack()
    {
        activeMelee = true;
        activeAbility = true;
        
        animator.Play("Melee", 0, 0);
        AbilityUIController.Instance.UseAbility(1, meleeAnimation.length);
        
        yield return new WaitForSeconds(3f / 24f);
        
        RaycastHit[] hits = Physics.SphereCastAll(cameraTransform.position, 1f, Vector3.forward, 1f, enemyLayer);
        foreach (RaycastHit hit in hits)
        {
            hit.collider.TryGetComponent(out Health health);
            health.TakeDamage(meleeDamage);
        }

        if (hits.Length > 0)
        {
            health.Heal(30f);
        }
        
        yield return new WaitForSeconds(meleeAnimation.length);
        
        AbilityUIController.Instance.CooldownAbility(1, meleeCooldown);
        activeAbility = false;
        
        yield return new WaitForSeconds(meleeCooldown);
        
        activeMelee = false;
    }
    
    private IEnumerator AltRangedAttack()
    {
        activeTrio = true;
        activeAbility = true;
        
        animator.Play("Fireball", 0, 0);
        AbilityUIController.Instance.UseAbility(5, fireballAnimation.length);

        yield return new WaitForSeconds(9f / 24f);

        Instantiate(trioball, leftHandObject.transform.position, cameraTransform.rotation);
        
        yield return new WaitForSeconds(fireballAnimation.length - (9f / 24f));
        
        AbilityUIController.Instance.CooldownAbility(5, trioballCooldown);
        activeAbility = false;
        
        yield return new WaitForSeconds(trioballCooldown);
        
        activeTrio = false;
    }

    private IEnumerator Fireball()
    {
        //Sets the fireball to active
        activeFireball = true;
        activeAbility = true;

        //Play animation & tell UI to highlight icon for the duration
        animator.Play("Fireball", 0, 0);
        AbilityUIController.Instance.UseAbility(2, fireballAnimation.length);
        
        //Waits for the action frame in the anim (actionFrameNumber / framerate)
        yield return new WaitForSeconds(9f / 24f);
        
        //Does the fireball code
        Instantiate(fireball, leftHandObject.transform.position, cameraTransform.rotation);
        
        //waits for the remainder of the animation
        yield return new WaitForSeconds(fireballAnimation.length - (9f / 24f));
        
        //Tells the UI to play the cooldown sequence
        AbilityUIController.Instance.CooldownAbility(2, fireballCooldown);
        activeAbility = false;
        
        //Waits for the cooldown of the ability
        yield return new WaitForSeconds(fireballCooldown);
        
        //Allows the fireball to be cast again
        activeFireball = false;
    }

    private IEnumerator PistolAttack()
    {
        animator.Play("Shoot", 1, 0);
        AbilityUIController.Instance.UseAbility(4, shootAnimation.length);

        activePistol = true;
        
        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out RaycastHit hit, 100f, enemyLayer + groundLayer) && hit.collider.TryGetComponent(out Health health))
        {
            health?.TakeDamage(pistolDamage);
        }
        
        yield return new WaitForSeconds(shootAnimation.length);
        
        activePistol = false;
    }

    private IEnumerator Dash()
    {
        //Play animation & tell UI to highlight icon for the duration
        //animator.Play("Dash", 0, 0);
        AbilityUIController.Instance.UseAbility(3, dashDuration);

        //Sets the dash to active
        activeDash = true;
        
        //Waits for the action frame in the anim (actionFrameNumber / framerate)
        //yield return new WaitForSeconds(9f / 24f);
        
        //Does the dash code
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        
        Vector3 direction = (transform.right * moveX + transform.forward * moveZ).normalized;

        dashVelocity = direction * dashForce;
        
        float dashTimer = dashDuration;
        while (dashTimer > 0f)
        {
            controller.Move(dashVelocity * Time.deltaTime);
            dashTimer -= Time.deltaTime;
            yield return null;
        }
        
        //waits for the remainder of the animation
        //yield return new WaitForSeconds(0.5f);
        
        //Tells the UI to play the cooldown sequence
        AbilityUIController.Instance.CooldownAbility(3, dashCooldown);
        
        //Waits for the cooldown of the ability
        yield return new WaitForSeconds(dashCooldown);
        
        //Allows the dash to be cast again
        activeDash = false;
    }
}
