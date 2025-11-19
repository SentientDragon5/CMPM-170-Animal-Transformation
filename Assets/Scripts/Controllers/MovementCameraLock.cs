
using Unity.Cinemachine;
using UnityEngine;

public class CinemachineFreeLookLock : MonoBehaviour
{
    [Header("Cinemachine FreeLook Camera")]
    public CinemachineCamera freeLookCamera;

    [Header("Lock Settings")]
    public bool lockOnMovement = true;
    public bool enableManualLock = true;
    public KeyCode toggleLockKey = KeyCode.L;

    [Header("Movement Detection")]
    public string horizontalInput = "Horizontal";
    public string verticalInput = "Vertical";
    public float inputThreshold = 0.1f;

    [Header("Lock Behavior")]
    public bool resetToForwardOnLock = true;
    public float lockSmoothness = 5f;

    // Cinemachine 3 components
    private CinemachineInputProvider inputProvider;
    private CinemachineCamera composer;
    private bool isLocked = false;
    private float targetXRotation = 0f;
    private float originalXAxisMaxSpeed;

    void Start()
    {
        // Auto-get CinemachineCamera if not assigned
        if (freeLookCamera == null)
        {
            freeLookCamera = GetComponent<CinemachineCamera>();
        }

        if (freeLookCamera == null)
        {
            Debug.LogError("Cinemachine Camera not found! Please assign it in the inspector.");
            return;
        }

        // Get the FreeLook composer component
        composer = freeLookCamera.GetComponent<CinemachineFreeLook>();
        if (composer == null)
        {
            Debug.LogError("CinemachineFreeLook component not found on the camera!");
            return;
        }

        // Get input provider
        inputProvider = freeLookCamera.GetComponent<CinemachineInputProvider>();

        // Store original settings
        originalXAxisMaxSpeed = composer.HorizontalAxis.MaxSpeed;

        Debug.Log("Cinemachine 3 FreeLook Lock initialized. Press 'L' to toggle lock.");
    }

    void Update()
    {
        if (freeLookCamera == null || composer == null) return;

        // Toggle lock with key
        if (enableManualLock && Input.GetKeyDown(toggleLockKey))
        {
            ToggleCameraLock();
        }

        // Auto-lock based on movement
        if (lockOnMovement)
        {
            HandleMovementLock();
        }

        // Apply lock if active
        if (isLocked)
        {
            ApplyCameraLock();
        }
    }

    void HandleMovementLock()
    {
        float horizontal = Input.GetAxis(horizontalInput);
        float vertical = Input.GetAxis(verticalInput);

        bool isMoving = Mathf.Abs(horizontal) > inputThreshold ||
                       Mathf.Abs(vertical) > inputThreshold;

        if (isMoving && !isLocked)
        {
            LockCamera();
        }
        else if (!isMoving && isLocked && lockOnMovement)
        {
            UnlockCamera();
        }
    }

    void ApplyCameraLock()
    {
        if (resetToForwardOnLock)
        {
            // Smoothly rotate camera to forward position
            if (inputProvider != null)
            {
                inputProvider.enabled = false;
            }

            // Smoothly reset X axis to forward
            float currentX = composer.HorizontalAxis.Value;
            targetXRotation = 0f; // Forward direction

            composer.HorizontalAxis.Value = Mathf.Lerp(
                currentX,
                targetXRotation,
                Time.deltaTime * lockSmoothness
            );
        }
        else
        {
            // Just stop receiving input
            if (inputProvider != null)
            {
                inputProvider.enabled = false;
            }
        }
    }

    public void ToggleCameraLock()
    {
        isLocked = !isLocked;

        if (isLocked)
        {
            LockCamera();
        }
        else
        {
            UnlockCamera();
        }

        Debug.Log($"Cinemachine FreeLook Lock: {(isLocked ? "ENABLED" : "DISABLED")}");
    }

    public void LockCamera()
    {
        if (freeLookCamera == null || composer == null) return;

        isLocked = true;

        // Store current rotation as target if not resetting to forward
        if (!resetToForwardOnLock)
        {
            targetXRotation = composer.HorizontalAxis.Value;
        }

        // Disable camera rotation input
        composer.HorizontalAxis.MaxSpeed = 0f;
        composer.VerticalAxis.MaxSpeed = 0f;

        // Disable input provider
        if (inputProvider != null)
        {
            inputProvider.enabled = false;
        }
    }

    public void UnlockCamera()
    {
        if (freeLookCamera == null || composer == null) return;

        isLocked = false;

        // Re-enable camera rotation (using reasonable defaults)
        composer.HorizontalAxis.MaxSpeed = originalXAxisMaxSpeed > 0 ? originalXAxisMaxSpeed : 300f;
        composer.VerticalAxis.MaxSpeed = 2f;

        // Re-enable input provider
        if (inputProvider != null)
        {
            inputProvider.enabled = true;
        }
    }

    // Public methods for external control
    public void SetLockState(bool locked)
    {
        if (locked)
        {
            LockCamera();
        }
        else
        {
            UnlockCamera();
        }
    }

    public bool IsLocked()
    {
        return isLocked;
    }

    // Called when the component is disabled
    void OnDisable()
    {
        if (freeLookCamera != null && !isLocked)
        {
            UnlockCamera();
        }
    }
}