using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SingleHealthBar : HealthBarUI
{
    [SerializeField] private Image fill;
    [SerializeField] private Image trail;          // optional "damage trail" behind the fill
    [SerializeField] private float fillDuration = 0.25f;
    [SerializeField] private float trailDelay = 0.3f;
    [SerializeField] private float trailDuration = 0.4f;

    private int maxHealth = 1;
    public override void Setup(int max, int current)
    {
        maxHealth = Mathf.Max(1, max);
        float value = GetFill(current);

        fill.DOKill();
        fill.fillAmount = value;

        if (trail != null)
        {
            trail.DOKill();
            trail.fillAmount = value;
        }
    }

    public override void SetHealth(int current)
    {
        float value = GetFill(current);

        fill.DOKill();
        fill.DOFillAmount(value, fillDuration);

        if (trail != null)
        {
            trail.DOKill();
            trail.DOFillAmount(value, trailDuration).SetDelay(trailDelay);
        }
    }
    private float GetFill(int current) => Mathf.Clamp01((float)current / maxHealth);
    private void OnDestroy()
    {
        if (fill != null) fill.DOKill();
        if (trail != null) trail.DOKill();
    }
}
