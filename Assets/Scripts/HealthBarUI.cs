using UnityEngine;

public abstract class HealthBarUI : MonoBehaviour
{
    public abstract void Setup(int maxHealth, int currentHealth);
    public abstract void SetHealth(int currentHealth);
}
