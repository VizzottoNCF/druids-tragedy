using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CombatantView : MonoBehaviour
{
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private StatusEffectsUI statusEffectsUI;
    public int MaxHealth { get; private set; }
    public int CurrentHealth { get; private set; }
    private Dictionary<StatusEffectType, int> statusEffects = new();
    public int Defense { get; set; }
    protected void SetupBase(int health, Sprite image, int defense = 0)
    {
        MaxHealth = health;
        CurrentHealth = MaxHealth;
        Defense = defense;
        spriteRenderer.sprite = image;
        UpdateHealthText();
    }
    public void UpdateDefense(int newDefense) => Defense = newDefense;
    private void UpdateHealthText() => healthText.text = $"HP: {CurrentHealth}";
    public void Damage(int damageAmount)
    {
        if (this == null) return;

        damageAmount -= Defense;

        if (damageAmount < 0) { damageAmount = 0; }

        CurrentHealth -= damageAmount;
        if (CurrentHealth < 0) { CurrentHealth = 0; }
        { transform.DOShakePosition(0.2f, 0.5f); }

        UpdateHealthText();
    }
    public void AddStatusEffect(StatusEffectType type, int stackCount)
    {
        if (statusEffects.ContainsKey(type))
        {
            statusEffects[type] += stackCount;
        }
        else
        {
            statusEffects.Add(type, stackCount);
        }
        statusEffectsUI.UpdateStatusEffectUI(type, GetStatusEffectStackCount(type));
    }
    public void RemoveStatusEffect(StatusEffectType type, int stackCount)
    {
        if (statusEffects.ContainsKey(type))
        {
            statusEffects[type] -= stackCount;
            if (statusEffects[type] <= 0)
            {
                statusEffects.Remove(type);
            }
            statusEffectsUI.UpdateStatusEffectUI(type, GetStatusEffectStackCount(type));
        }
    }
    public int GetStatusEffectStackCount(StatusEffectType type)
    {
        return statusEffects.ContainsKey(type) ? statusEffects[type] : 0;
    }
}
