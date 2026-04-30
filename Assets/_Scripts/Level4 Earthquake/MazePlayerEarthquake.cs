using UnityEngine;

public class MazePlayerEarthquake : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 6f;
    public float acceleration = 18f;   // 🔥 how fast you reach speed
    public float deceleration = 22f;   // 🔥 how fast you stop

    [Header("Input")]
    public VirtualJoystick joystick;

    [Header("Control Lock")]
    public bool canMove = true;
    public bool inputLocked = false;

    [Header("Stop Feel")]
    public float stopSmoothTime = 0.35f;

    private Rigidbody2D rb;

    private Vector2 currentVelocity;   // 🔥 smooth velocity system
    private Vector2 inputVector;

    private float stopTimer = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (joystick == null)
            joystick = FindFirstObjectByType<VirtualJoystick>();
    }

    void FixedUpdate()
    {
        // =========================
        // INPUT LOCKED → SMOOTH STOP
        // =========================
        if (!canMove || inputLocked)
        {
            stopTimer += Time.fixedDeltaTime;

            float t = stopTimer / stopSmoothTime;
            float decay = EaseOutCubic(Mathf.Clamp01(t));

            currentVelocity = Vector2.Lerp(currentVelocity, Vector2.zero, decay);

            rb.linearVelocity = currentVelocity;
            return;
        }

        // =========================
        // READ INPUT (smoothed feel)
        // =========================
        stopTimer = 0f;

        float x = joystick != null ? joystick.Horizontal() : 0f;
        float y = joystick != null ? joystick.Vertical() : 0f;

        inputVector = new Vector2(x, y);

        Vector2 targetVelocity = inputVector * moveSpeed;

        // =========================
        // ACCELERATION (cartoon feel)
        // =========================
        currentVelocity = Vector2.MoveTowards(
            currentVelocity,
            targetVelocity,
            acceleration * Time.fixedDeltaTime
        );

        // =========================
        // FRICTION WHEN NO INPUT
        // =========================
        if (inputVector.sqrMagnitude < 0.01f)
        {
            currentVelocity = Vector2.MoveTowards(
                currentVelocity,
                Vector2.zero,
                deceleration * Time.fixedDeltaTime
            );
        }

        rb.linearVelocity = currentVelocity;

        // =========================
        // ROTATION (only when moving)
        // =========================
        if (currentVelocity.sqrMagnitude > 0.01f)
        {
            float angle = Mathf.Atan2(currentVelocity.y, currentVelocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
        }
    }

    // =========================
    // CONTROL API
    // =========================

    public void LockInput()
    {
        inputLocked = true;
        stopTimer = 0f;
    }

    public void UnlockInput()
    {
        inputLocked = false;
        stopTimer = 0f;
    }

    public void DisableMovement()
    {
        canMove = false;
        stopTimer = 0f;
    }

    public void EnableMovement()
    {
        canMove = true;
    }

    public void HardStop()
    {
        currentVelocity = Vector2.zero;
        rb.linearVelocity = Vector2.zero;
    }

    // =========================
    // EASING
    // =========================
    float EaseOutCubic(float t)
    {
        return 1f - Mathf.Pow(1f - t, 3f);
    }
}