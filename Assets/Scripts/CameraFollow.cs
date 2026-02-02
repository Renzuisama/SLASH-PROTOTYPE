using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;
    [SerializeField] private bool autoFindPlayer = true;

    [Header("Follow Settings")]
    [SerializeField] private bool followX = true;
    [SerializeField] private bool followY = true;
    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField] private Vector3 offset = new Vector3(0f, 0f, -10f);

    [Header("Bounds (Optional)")]
    [SerializeField] private bool useBounds = false;
    [SerializeField] private float minX = -50f;
    [SerializeField] private float maxX = 50f;
    [SerializeField] private float minY = -50f;
    [SerializeField] private float maxY = 50f;

    [Header("Dead Zone (Optional)")]
    [SerializeField] private bool useDeadZone = false;
    [SerializeField] private float deadZoneRadius = 1f;

    [Header("Look Ahead (Optional)")]
    [SerializeField] private bool useLookAhead = false;
    [SerializeField] private float lookAheadDistance = 2f;
    [SerializeField] private float lookAheadSpeed = 2f;

    private Vector3 velocity = Vector3.zero;
    private Vector3 currentLookAhead = Vector3.zero;

    private void Start()
    {
        // Auto-find player if not assigned
        if (target == null && autoFindPlayer)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
                Debug.Log("CameraFollow: Auto-found player");
            }
            else
            {
                Debug.LogWarning("CameraFollow: Could not find player! Make sure player has 'Player' tag.");
            }
        }

        // Set initial position
        if (target != null)
        {
            Vector3 initialPos = target.position + offset;
            transform.position = new Vector3(initialPos.x, initialPos.y, transform.position.z);
        }
    }

    private void LateUpdate()
    {
        if (target == null) return;

        // Calculate desired position
        Vector3 targetPosition = target.position + offset;

        // Apply look-ahead
        if (useLookAhead)
        {
            Rigidbody2D targetRb = target.GetComponent<Rigidbody2D>();
            if (targetRb != null)
            {
                Vector3 lookAhead = (Vector3)targetRb.linearVelocity.normalized * lookAheadDistance;
                currentLookAhead = Vector3.Lerp(currentLookAhead, lookAhead, Time.deltaTime * lookAheadSpeed);
                targetPosition += currentLookAhead;
            }
        }

        // Apply dead zone
        if (useDeadZone)
        {
            Vector3 currentPosNoZ = new Vector3(transform.position.x, transform.position.y, 0f);
            Vector3 targetPosNoZ = new Vector3(targetPosition.x, targetPosition.y, 0f);
            float distance = Vector3.Distance(currentPosNoZ, targetPosNoZ);

            if (distance < deadZoneRadius)
            {
                // Stay in current position if within dead zone
                return;
            }
        }

        // Calculate smoothed position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);

        // Apply axis following
        float newX = followX ? smoothedPosition.x : transform.position.x;
        float newY = followY ? smoothedPosition.y : transform.position.y;
        float newZ = offset.z; // Always use offset Z for 2D

        // Apply bounds
        if (useBounds)
        {
            newX = Mathf.Clamp(newX, minX, maxX);
            newY = Mathf.Clamp(newY, minY, maxY);
        }

        // Set camera position
        transform.position = new Vector3(newX, newY, newZ);
    }

    /// <summary>
    /// Set a new target for the camera
    /// </summary>
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    /// <summary>
    /// Instantly snap camera to target (no smoothing)
    /// </summary>
    public void SnapToTarget()
    {
        if (target == null) return;

        Vector3 targetPosition = target.position + offset;
        float newX = followX ? targetPosition.x : transform.position.x;
        float newY = followY ? targetPosition.y : transform.position.y;

        if (useBounds)
        {
            newX = Mathf.Clamp(newX, minX, maxX);
            newY = Mathf.Clamp(newY, minY, maxY);
        }

        transform.position = new Vector3(newX, newY, offset.z);
    }

    /// <summary>
    /// Set camera bounds based on tilemap
    /// </summary>
    public void SetBoundsFromTilemap(Bounds tilemapBounds, float cameraHalfHeight, float cameraHalfWidth)
    {
        useBounds = true;
        minX = tilemapBounds.min.x + cameraHalfWidth;
        maxX = tilemapBounds.max.x - cameraHalfWidth;
        minY = tilemapBounds.min.y + cameraHalfHeight;
        maxY = tilemapBounds.max.y - cameraHalfHeight;
    }

    private void OnDrawGizmosSelected()
    {
        if (target != null)
        {
            // Draw line to target
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, target.position);

            // Draw dead zone
            if (useDeadZone)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(transform.position, deadZoneRadius);
            }
        }

        // Draw bounds
        if (useBounds)
        {
            Gizmos.color = Color.red;
            Vector3 topLeft = new Vector3(minX, maxY, 0f);
            Vector3 topRight = new Vector3(maxX, maxY, 0f);
            Vector3 bottomLeft = new Vector3(minX, minY, 0f);
            Vector3 bottomRight = new Vector3(maxX, minY, 0f);

            Gizmos.DrawLine(topLeft, topRight);
            Gizmos.DrawLine(topRight, bottomRight);
            Gizmos.DrawLine(bottomRight, bottomLeft);
            Gizmos.DrawLine(bottomLeft, topLeft);
        }
    }
}
