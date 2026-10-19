using UnityEngine;
using UnityEngine.InputSystem;

public class CameraDrag : MonoBehaviour
{
    [SerializeField] private float lowerBound = -4.5f;
    [SerializeField] private float upperBound = -7f;
    private Camera cam;
    private Vector3 origin, difference;

    private bool isDragging;

    private void Start()
    {
        cam = Camera.main;

        transform.position = new Vector3(0f, lowerBound, -10f); // start at the bottom of the map
    }
    public void OnDrag(InputAction.CallbackContext ctx)
    {
        if (ctx.started) { origin = GetCameraPos(Input.mousePosition); }

        isDragging = ctx.started || ctx.performed;
    }

    private void LateUpdate()
    {
        if (!isDragging) { return; }

        difference = GetCameraPos(Input.mousePosition) - transform.position;

        Vector3 newPos = origin - difference;

        // bounds so camera stays locked in the middle and can't go too high or too low
        newPos.x = 0f;
        newPos.z = -10f;

        if (newPos.y < lowerBound) { newPos.y = lowerBound; }
        if (newPos.y > upperBound) { newPos.y = upperBound; }

        transform.position = newPos;
    }

    private Vector3 GetCameraPos(Vector3 mousePos) => Camera.main.ScreenToWorldPoint(mousePos);
}