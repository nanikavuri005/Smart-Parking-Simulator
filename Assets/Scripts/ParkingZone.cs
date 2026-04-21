using UnityEngine;

/// <summary>
/// Detects whether the player is inside the parking area and nearly stationary.
/// Attach to a Parking Zone GameObject with a Trigger Collider.
/// </summary>
public class ParkingZone : MonoBehaviour
{
    [Tooltip("Maximum allowed speed to count as parked")]
    public float requiredStopSpeed = 0.35f;

    [Tooltip("How long the car must stay below required speed")]
    public float settleDuration = 0.8f;

    private float stillTimer;

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        Rigidbody carRb = other.attachedRigidbody;
        if (carRb == null)
        {
            return;
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnParkingInProgress();
        }

        // Count how long the car remains almost stopped.
        if (carRb.velocity.magnitude <= requiredStopSpeed)
        {
            stillTimer += Time.deltaTime;

            if (stillTimer >= settleDuration && GameManager.Instance != null)
            {
                GameManager.Instance.OnParkedSuccessfully();
            }
        }
        else
        {
            stillTimer = 0f;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        stillTimer = 0f;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnParkingInProgress();
        }
    }

    public void ResetZone()
    {
        stillTimer = 0f;
    }
}
