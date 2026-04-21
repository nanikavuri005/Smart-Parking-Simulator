using UnityEngine;

/// <summary>
/// Handles player car movement with Rigidbody physics.
/// Attach to the player car GameObject.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class CarController : MonoBehaviour
{
    [Header("Movement")]
    [Tooltip("Forward acceleration when pressing W / Up Arrow")]
    public float forwardAcceleration = 12f;

    [Tooltip("Reverse acceleration when pressing S / Down Arrow")]
    public float reverseAcceleration = 8f;

    [Tooltip("Maximum speed in meters per second")]
    public float maxSpeed = 14f;

    [Header("Steering")]
    [Tooltip("How fast the car rotates while moving")]
    public float steeringSensitivity = 90f;

    [Tooltip("Higher values make steering response snappier")]
    public float steeringSmoothness = 8f;

    private Rigidbody rb;
    private float throttleInput;
    private float steerInput;
    private float currentSteer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // Recommended Rigidbody settings for stable car-like behavior.
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    private void Update()
    {
        // Supports both WASD and Arrow keys through Unity's default axes.
        throttleInput = Input.GetAxis("Vertical");
        steerInput = Input.GetAxis("Horizontal");
    }

    private void FixedUpdate()
    {
        HandleMovement();
        HandleSteering();
    }

    private void HandleMovement()
    {
        float acceleration = throttleInput >= 0f ? forwardAcceleration : reverseAcceleration;

        // Add force in the car's forward direction.
        rb.AddForce(transform.forward * throttleInput * acceleration, ForceMode.Acceleration);

        // Clamp top speed to keep controls predictable.
        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }
    }

    private void HandleSteering()
    {
        // Smooth steering input so turning feels less abrupt.
        currentSteer = Mathf.Lerp(currentSteer, steerInput, steeringSmoothness * Time.fixedDeltaTime);

        // Turn amount scales with current speed so steering at standstill is limited.
        float speedFactor = Mathf.Clamp01(rb.velocity.magnitude / 2f);
        float turnAngle = currentSteer * steeringSensitivity * speedFactor * Time.fixedDeltaTime;

        Quaternion turnRotation = Quaternion.Euler(0f, turnAngle, 0f);
        rb.MoveRotation(rb.rotation * turnRotation);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Report crash to GameManager so UI can react.
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RegisterCrash();
        }

        Debug.Log($"Crash detected with: {collision.gameObject.name}");
    }
}
