using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public enum TutorialTarget { None, Hand, Tray, Mana, EndTurn, Enemy, Hero, QTE }
public class TutorialUI : MonoBehaviour
{
    [System.Serializable]
    private struct TargetEntry
    {
        public TutorialTarget target;
        public Transform anchor;   // world object or UI element in the Combat scene
    }

    [SerializeField] private GameObject box;
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private RectTransform arrow;
    [SerializeField] private List<TargetEntry> targets = new();
    [SerializeField] private float arrowOffsetY = 80f;

    private Action onContinue;
    private bool waitingForClick;
    private float clickReadyTime;
    private Tween arrowTween;

    private void Awake() => Hide();

    public void Show(string message, bool waitForClick, TutorialTarget highlight, Action continueCallback)
    {
        box.SetActive(true);
        messageText.text = message;

        // fade/scale in
        box.transform.localScale = Vector3.one * 0.9f;
        box.transform.DOScale(1f, 0.15f).SetLink(box);

        waitingForClick = waitForClick;
        onContinue = continueCallback;

        // small delay so the click that finished the previous step doesn't skip this one
        clickReadyTime = Time.unscaledTime + 0.3f;

        PointAt(highlight);
    }

    public void Hide()
    {
        waitingForClick = false;
        onContinue = null;
        box.SetActive(false);
        StopArrow();
        arrow.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!waitingForClick || Time.unscaledTime < clickReadyTime) return;

        if (Input.GetMouseButtonDown(0))
        {
            waitingForClick = false;
            var cb = onContinue;
            onContinue = null;
            cb?.Invoke();
        }
    }

    private void PointAt(TutorialTarget target)
    {
        StopArrow();

        Transform anchor = FindAnchor(target);
        if (target == TutorialTarget.None || anchor == null)
        {
            arrow.gameObject.SetActive(false);
            return;
        }

        arrow.gameObject.SetActive(true);

        // world position -> screen position (works for world objects; for UI elements in an
        // Overlay canvas, anchor.position is already a screen position, so skip the conversion)
        Vector3 screenPos = anchor is RectTransform
            ? anchor.position
            : Camera.main.WorldToScreenPoint(anchor.position);

        arrow.position = screenPos + Vector3.up * arrowOffsetY;

        // bounce up and down
        arrowTween = arrow.DOAnchorPosY(arrow.anchoredPosition.y + 20f, 0.5f)
                          .SetLoops(-1, LoopType.Yoyo)
                          .SetEase(Ease.InOutSine)
                          .SetLink(arrow.gameObject);
    }

    private Transform FindAnchor(TutorialTarget t)
    {
        foreach (var e in targets) if (e.target == t) return e.anchor;
        return null;
    }

    private void StopArrow()
    {
        arrowTween?.Kill();
        arrowTween = null;
    }
}