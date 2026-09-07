using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Настройки движения")]
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float jumpHeight = 1.5f;
    public float gravity = -9.81f;

    [Header("Поворот")]
    public bool rotateToMovement = true;
    public float rotationSpeed = 10f;

    [Header("Режим движения")]
    public bool useCameraRelative = true;

    private CharacterController controller;
    private Transform cameraTransform;
    private Vector3 velocity;
    private bool isGrounded;

    // -------- Данные для анимации --------
    public float CurrentSpeed { get; private set; }
    public bool IsGrounded { get; private set; }
    public bool JumpTriggered { get; private set; }
    public bool IsRunning { get; private set; }
    public Vector3 MoveDirection { get; private set; }

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        FindCamera();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindCamera();
    }

    private void FindCamera()
    {
        if (useCameraRelative && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
        else
        {
            cameraTransform = null;
        }
    }

    private void Update()
    {
        if (controller == null || !controller.enabled) return;

        HandleGroundCheck();
        HandleMovementAndGravity();
        HandleJump();
        UpdateAnimationData();
    }

    private void HandleGroundCheck()
    {
        isGrounded = controller.isGrounded;
        IsGrounded = isGrounded;

        if (isGrounded && velocity.y < 0f)
        {
            velocity.y = -2f;
        }
    }

    private void HandleMovementAndGravity()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 moveDirection = Vector3.zero;

        if (useCameraRelative && cameraTransform != null)
        {
            Vector3 forward = cameraTransform.forward;
            Vector3 right = cameraTransform.right;

            forward.y = 0f;
            right.y = 0f;

            forward.Normalize();
            right.Normalize();

            moveDirection = (forward * vertical + right * horizontal).normalized;
        }
        else
        {
            moveDirection = new Vector3(horizontal, 0f, vertical).normalized;
        }

        MoveDirection = moveDirection;

        IsRunning = Input.GetKey(KeyCode.LeftShift) && moveDirection.magnitude > 0.1f;
        float currentSpeed = IsRunning ? runSpeed : walkSpeed;

        if (rotateToMovement && moveDirection.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }

        velocity.y += gravity * Time.deltaTime;

        Vector3 finalMove = moveDirection * currentSpeed;
        finalMove.y = velocity.y;

        controller.Move(finalMove * Time.deltaTime);
    }

    private void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            JumpTriggered = true;
        }
        else
        {
            JumpTriggered = false;
        }
    }

    private void UpdateAnimationData()
    {
        if (MoveDirection.magnitude > 0.1f)
        {
            CurrentSpeed = IsRunning ? 1f : 0.5f;
        }
        else
        {
            CurrentSpeed = 0f;
        }
    }

    public void ResetJumpTrigger()
    {
        JumpTriggered = false;
    }
}