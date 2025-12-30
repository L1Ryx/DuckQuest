using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerControl : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private float inputDeadzone = 0.15f;
    [SerializeField] private float movingVelocityThreshold = 0.001f;

    private Rigidbody2D rb;

    private Vector2 moveInput;
    private Vector2 moveVector;

    // 0=Down, 1=Up, 2=Left, 3=Right
    private int facing = 0;

    // 0=none, 1=vertical, 2=horizontal
    private int lockedAxis = 0;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;

        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }

    private void FixedUpdate()
    {
        moveVector = moveInput.sqrMagnitude > 1f ? moveInput.normalized : moveInput;
        rb.linearVelocity = moveVector * moveSpeed;
    }

    private void Update()
    {
        UpdateFacingLock(moveInput);
    }

    private void LateUpdate()
    {
        bool isMoving = rb.linearVelocity.sqrMagnitude > movingVelocityThreshold;
        animator.SetBool("IsMoving", isMoving);
        animator.SetInteger("Facing", facing);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    private void UpdateFacingLock(Vector2 input)
    {
        float x = input.x;
        float y = input.y;

        bool hasX = Mathf.Abs(x) > inputDeadzone;
        bool hasY = Mathf.Abs(y) > inputDeadzone;

        if (lockedAxis == 0)
        {
            if (!hasX && !hasY) return;

            if (hasY && (!hasX || Mathf.Abs(y) >= Mathf.Abs(x)))
                lockedAxis = 1;
            else
                lockedAxis = 2;
        }

        if (lockedAxis == 1)
        {
            if (hasY)
            {
                facing = (y > 0f) ? 1 : 0;
                return;
            }

            lockedAxis = 0;
            UpdateFacingLock(input);
            return;
        }

        if (lockedAxis == 2)
        {
            if (hasX)
            {
                facing = (x > 0f) ? 3 : 2;
                return;
            }

            lockedAxis = 0;
            UpdateFacingLock(input);
        }
    }
}
