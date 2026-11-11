// SegmentedHealthBar.cs
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class SegmentedHealthBar : HealthBarUI
{
    [SerializeField] private HealthSegment segmentPrefab;
    [SerializeField] private Transform container;
    [SerializeField] private int healthPerSegment = 50;
    [SerializeField] private float fillDuration = 0.25f;
    [SerializeField] private float trailDelay = 0.3f;
    [SerializeField] private float trailDuration = 0.4f;

    private readonly List<HealthSegment> segments = new();

    public override void Setup(int max, int current)
    {
        foreach (var s in segments)
        {
            if (s == null) continue;
            s.fill.DOKill();
            Destroy(s.gameObject);
        }
        segments.Clear();

        Apply(current, true);
    }

    public override void SetHealth(int current) => Apply(current, animate: true);
    private void Apply(int current, bool animate)
    {
        current = Mathf.Max(0, current);
        int needed = Mathf.CeilToInt(current / (float)healthPerSegment);

        // add new segments when gained HP
        while (segments.Count < needed)
        {
            HealthSegment seg = Instantiate(segmentPrefab, container);
            seg.fill.fillAmount = 0f;
            segments.Add(seg);
        }

        // drain the extra segments from the end, then remove them
        while (segments.Count > needed)
        {
            HealthSegment seg = segments[segments.Count - 1];
            segments.RemoveAt(segments.Count - 1);

            seg.fill.DOKill();
            if (animate)
            {
                seg.fill.DOFillAmount(0f, fillDuration)
                    .SetLink(seg.gameObject)
                    .OnComplete(() => Destroy(seg.gameObject));
            }
            else Destroy(seg.gameObject);
        }

        // fill remaining HP segments
        for (int i = 0; i < segments.Count; i++)
        {
            float target = Mathf.Clamp01((current - i * healthPerSegment) / (float)healthPerSegment);

            segments[i].fill.DOKill();
            segments[i].trail.DOKill();
            if (animate)
            {
                segments[i].fill.DOFillAmount(target, fillDuration).SetLink(segments[i].gameObject);
                segments[i].trail.DOFillAmount(target, trailDuration).SetDelay(trailDelay).SetLink(segments[i].gameObject);
            }
            else segments[i].fill.fillAmount = target;
        }
    }
}