using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls game state, UI messages, restart behavior, timer, and parking arrow indicator.
/// Attach to an empty GameObject named GameManager.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("References")]
    public Transform carTransform;
    public Rigidbody carRigidbody;
    public ParkingZone parkingZone;

    [Header("UI")]
    public Text statusText;
    public Text timerText;

    [Header("Optional Arrow Indicator")]
    [Tooltip("Arrow object that will rotate toward parkingSpot")]
    public Transform arrowIndicator;
    public Transform parkingSpot;

    private Vector3 startPosition;
    private Quaternion startRotation;
    private bool parkedSuccessfully;
    private float elapsedTime;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        if (carTransform != null)
        {
            startPosition = carTransform.position;
            startRotation = carTransform.rotation;
        }

        SetStatus("Parking...");
        UpdateTimerUI();
    }

    private void Update()
    {
        // Timer runs until player parks successfully.
        if (!parkedSuccessfully)
        {
            elapsedTime += Time.deltaTime;
            UpdateTimerUI();
        }

        // Press R to reset the car and state.
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetCar();
        }

        UpdateArrowIndicator();
    }

    public void OnParkingInProgress()
    {
        if (!parkedSuccessfully)
        {
            SetStatus("Parking...");
        }
    }

    public void OnParkedSuccessfully()
    {
        if (parkedSuccessfully)
        {
            return;
        }

        parkedSuccessfully = true;
        SetStatus("Parked Successfully");
    }

    public void RegisterCrash()
    {
        if (!parkedSuccessfully)
        {
            SetStatus("Crash Detected");
        }
    }

    private void ResetCar()
    {
        if (carTransform == null)
        {
            return;
        }

        carTransform.position = startPosition;
        carTransform.rotation = startRotation;

        if (carRigidbody != null)
        {
            carRigidbody.velocity = Vector3.zero;
            carRigidbody.angularVelocity = Vector3.zero;
        }

        parkedSuccessfully = false;
        elapsedTime = 0f;

        if (parkingZone != null)
        {
            parkingZone.ResetZone();
        }

        SetStatus("Parking...");
        UpdateTimerUI();
    }

    private void UpdateTimerUI()
    {
        if (timerText == null)
        {
            return;
        }

        timerText.text = $"Time: {elapsedTime:F1}s";
    }

    private void SetStatus(string message)
    {
        if (statusText != null)
        {
            statusText.text = message;
        }
    }

    private void UpdateArrowIndicator()
    {
        if (arrowIndicator == null || parkingSpot == null || carTransform == null)
        {
            return;
        }

        arrowIndicator.gameObject.SetActive(!parkedSuccessfully);

        Vector3 direction = parkingSpot.position - carTransform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude > 0.001f)
        {
            arrowIndicator.rotation = Quaternion.LookRotation(direction);
        }
    }
}
