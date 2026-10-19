using TMPro;
using UnityEngine;

public class EnemyView : CombatantView
{
    [SerializeField] private TMP_Text attackText;
    public int AttackPower { get; set; }
    public EnemyBehavior Behavior { get; private set; }
    public int TurnCount { get; set; }

    public void Setup(EnemyData enemyData)
    {
        AttackPower = enemyData.AttackPower;
        Behavior = enemyData.Behavior;
        TurnCount = 0;
        UpdateAttackText();
        SetupBase(enemyData.Health, enemyData.Image);
    }
    private void UpdateAttackText() => attackText.text = $"ATK: {AttackPower}";
}
