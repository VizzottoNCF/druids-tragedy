using UnityEngine;
using UnityEngine.InputSystem;

public class CameraDrag : MonoBehaviour
{
    [Header("Bounds")]
    [SerializeField] private float lowerBound = -4.5f;
    [SerializeField] private float upperBound = -7f;
    private Camera cam;
    private Vector3 origin, difference;

    private bool isDragging;

    [Header("Scroll")]
    [SerializeField] private float scrollSpeed = 1f;
    [SerializeField] private float smoothTime = 0.1f;

    private float targetY;
    private float velocity;

    private void Start()
    {
        float min = Mathf.Min(lowerBound, upperBound);
        float max = Mathf.Max(lowerBound, upperBound);

        // use the saved position if there is one, otherwise start at the bottom of the map
        float saved = MatchData.Instance != null && MatchData.Instance.PathwayCameraY.HasValue ? MatchData.Instance.PathwayCameraY.Value : lowerBound;

        targetY = Mathf.Clamp(saved, min, max);
        transform.position = new Vector3(0f, targetY, -10f);
    }
    private void Update()
    {
        float scroll = Input.mouseScrollDelta.y;
        if (scroll != 0f)
        {
            targetY += scroll * scrollSpeed;

            float min = Mathf.Min(lowerBound, upperBound);
            float max = Mathf.Max(lowerBound, upperBound);
            targetY = Mathf.Clamp(targetY, min, max);

            // save targetY to MatchData so it can be restored when returning to the map
            if (MatchData.Instance != null) MatchData.Instance.PathwayCameraY = targetY;
        }

        float y = smoothTime > 0f ? Mathf.SmoothDamp(transform.position.y, targetY, ref velocity, smoothTime) : targetY;

        transform.position = new Vector3(0f, y, -10f);
    }

    private Vector3 GetCameraPos(Vector3 mousePos) => Camera.main.ScreenToWorldPoint(mousePos);
}