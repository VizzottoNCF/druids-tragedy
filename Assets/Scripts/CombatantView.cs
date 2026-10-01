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
    protected void SetupBase(int health, Sprite image)
    {
        MaxHealth = health;
        CurrentHealth = MaxHealth;
        spriteRenderer.sprite = image;
        UpdateHealthText();
    }
    protected void SetupBase(int health, int currentHealth, Sprite image)
    {
        MaxHealth = health;
        CurrentHealth = currentHealth;
        spriteRenderer.sprite = image;
        UpdateHealthText();
    }
    public int GetCurrentHealth() => CurrentHealth;
    private void UpdateHealthText() => healthText.text = $"HP: {CurrentHealth}";
    public void Damage(int damageAmount)
    {
        if (this == null) return;

        // subtract armor from damage amount
        int remainingDamage = damageAmount;
        int currentArmor = GetStatusEffectStackCount(StatusEffectType.ARMOR);
        if (currentArmor > 0)
        {
            if (currentArmor >= damageAmount)
            {
                RemoveStatusEffect(StatusEffectType.ARMOR, remainingDamage);
                remainingDamage = 0;
            }
            else if (currentArmor < damageAmount)
            {
                RemoveStatusEffect(StatusEffectType.ARMOR, currentArmor);
                remainingDamage -= currentArmor;
            }
        }

        // apply remaining damage to health
        if (remainingDamage > 0)
        {
            CurrentHealth -= remainingDamage;
            if (CurrentHealth < 0) { CurrentHealth = 0; }
            { transform.DOShakePosition(0.2f, 0.5f); }
        }
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
