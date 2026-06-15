using UnityEngine;

public class CameraFllow : Singleton<CameraFllow>
{
    [Header("Target")]
    public Rigidbody2D target;

    [Header("Follow Settings")]
    [SerializeField] private float smoothTime = 0.12f;
    [SerializeField] private float lookAheadDistance = 1.5f;
    [SerializeField] private float lookAheadSmooth = 0.2f;

    [Header("Bounds")]
    public bool useBounds = true;
    public Vector2 minBounds;
    public Vector2 maxBounds;

    private Vector3 velocity;
    private float currentLookAhead;
    private float lookAheadVelocity;

    private float camHalfHeight;
    private float camHalfWidth;
    private bool checkMap = false;
    public bool checkcam => checkMap;

    void Start()
    {
        Camera cam = GetComponent<Camera>();

        camHalfHeight = cam.orthographicSize;
        camHalfWidth = camHalfHeight * cam.aspect;
    }

    void LateUpdate()
    {
        if (target == null) return;

        HandleLookAhead();

        Vector3 targetPosition = new Vector3(
            target.position.x + currentLookAhead,
            target.position.y,
            transform.position.z
        );

        if (useBounds)
        {
            float minX = minBounds.x + camHalfWidth;
            float maxX = maxBounds.x - camHalfWidth;
            float minY = minBounds.y + camHalfHeight;
            float maxY = maxBounds.y - camHalfHeight;

            // Nếu room nhỏ hơn camera → khóa giữa
            if (minX > maxX)
            {
                targetPosition.x = (minBounds.x + maxBounds.x) * 0.5f;  
            }    
            else
            {

               float clampedX = Mathf.Clamp(targetPosition.x, minX, maxX);
    if (clampedX != targetPosition.x)
        checkMap = true;

    targetPosition.x = clampedX;
               
            }
            if (minY > maxY)
                targetPosition.y = (minBounds.y + maxBounds.y) * 0.5f;
            else
                targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);
        }

        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref velocity,
            smoothTime
        );
    }

    void HandleLookAhead()
    {
        if (Mathf.Abs(target.velocity.x) > 0.1f)
        {
            float direction = Mathf.Sign(target.velocity.x);
            float targetLook = direction * lookAheadDistance;

            currentLookAhead = Mathf.SmoothDamp(
                currentLookAhead,
                targetLook,
                ref lookAheadVelocity,
                lookAheadSmooth
            );
        }
        else
        {
            currentLookAhead = Mathf.SmoothDamp(
                currentLookAhead,
                0f,
                ref lookAheadVelocity,
                lookAheadSmooth
            );
        }
    }

    // Cho phép đổi bounds bằng code nếu muốn
    public void SetBounds(Vector2 min, Vector2 max)
    {
        minBounds = min;
        maxBounds = max;
    }
}