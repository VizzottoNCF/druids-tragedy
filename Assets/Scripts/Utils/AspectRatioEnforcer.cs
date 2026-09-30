using UnityEngine;

public class AspectRatioEnforcer : MonoBehaviour
{
    private Camera cam;
    private float targetAspectRatio = 16f / 9f;
    private float lastWidth;
    private float lastHeight;

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    private void SetCameraAspectRatio()
    {
        float windowAspect = (float)Screen.width / Screen.height;
        float scaleHeight = windowAspect / targetAspectRatio;

        if (scaleHeight < 1f)
        {
            // letterbox
            Rect rect = cam.rect;

            rect.width = 1f;
            rect.height = scaleHeight;
            rect.x = 0;
            rect.y = (1f - scaleHeight) / 2f;

            cam.rect = rect;
        }
        else
        {
            // pillerbox
            float scaleWidth = 1.0f / scaleHeight;

            Rect rect = cam.rect;

            rect.width = scaleWidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scaleWidth) / 2.0f;
            rect.y = 0;

            cam.rect = rect;
        }
    }

    private void Update()
    {
        SetCameraAspectRatio();
    }
}
