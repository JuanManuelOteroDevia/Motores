using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    public float moveSpeed = 5f;
    public float runSpeed = 8f;
    public float jumpForce = 7f;

    [Header("Cámara")]
    public Transform cameraTransform;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundMask = ~0;

    [Header("Sonido de pasos")]
    public AudioClip footstepClip;
    public float footstepInterval = 0.4f;

    private Rigidbody rb;
    private Animator animator;
    private bool wantsToJump = false;
    private bool wasGrounded = true;

    private AudioSource audioSource;
    private float footstepTimer = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        animator = GetComponent<Animator>();
        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Update()
    {
        
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        Vector3 moveDir = new Vector3(moveX, 0, moveZ).normalized;
        bool moving = moveDir.magnitude > 0.1f;
        bool isGrounded = CheckGrounded();

        
        if (moving && isGrounded)
        {
            footstepTimer += Time.deltaTime;
            if (footstepTimer >= footstepInterval)
            {
                if (footstepClip != null)
                    audioSource.PlayOneShot(footstepClip);
                footstepTimer = 0f;
            }
        }
        else
        {
            footstepTimer = 0f;
        }

        
        bool isRunning = (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                         && moving && isGrounded;

        
        if (animator != null)
        {
            animator.SetBool("IsRunning", moving && isRunning);
            animator.SetBool("IsWalking", moving && isGrounded && !isRunning);
            animator.SetBool("IsIdle", !moving && isGrounded &&
                                   !animator.GetBool("IsJumping") &&
                                   !animator.GetBool("IsFalling"));
        }

        
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            wantsToJump = true;

        
        if (!isGrounded && rb.velocity.y < -0.15f)
        {
            animator?.SetBool("IsFalling", true);
            animator?.SetBool("IsJumping", false);
        }

        
        if (!wasGrounded && isGrounded)
        {
            animator?.SetBool("IsFalling", false);
            animator?.SetBool("IsJumping", false);
        }
        wasGrounded = isGrounded;

        
        if (moving && cameraTransform)
        {
            Vector3 camF = cameraTransform.forward; camF.y = 0; camF.Normalize();
            Vector3 camR = cameraTransform.right; camR.y = 0; camR.Normalize();
            Vector3 desiredDir = (camF * moveZ + camR * moveX).normalized;

            if (desiredDir.sqrMagnitude > 0.001f)
                transform.rotation = Quaternion.LookRotation(desiredDir);
        }

        
        if (cameraTransform)
        {
            Vector3 camF = cameraTransform.forward; camF.y = 0; camF.Normalize();
            Vector3 camR = cameraTransform.right; camR.y = 0; camR.Normalize();

            float currentSpeed = isRunning ? runSpeed : moveSpeed;
            Vector3 offset = (camF * moveZ + camR * moveX).normalized
                             * currentSpeed * Time.deltaTime;
            rb.MovePosition(rb.position + offset);
        }
    }

    void FixedUpdate()
    {
        if (wantsToJump && CheckGrounded())
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
            animator?.SetBool("IsJumping", true);
            animator?.SetBool("IsIdle", false);
            animator?.SetBool("IsWalking", false);
            wantsToJump = false;
        }
    }

    private bool CheckGrounded()
    {
        if (groundCheck != null)
            return Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundMask);
        else
            return Physics.Raycast(transform.position + Vector3.up * 0.1f,
                                   Vector3.down, 0.35f, groundMask);
    }
}
