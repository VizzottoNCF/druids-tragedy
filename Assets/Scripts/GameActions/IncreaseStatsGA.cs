using UnityEngine;

public class IncreaseStatsGA
{
    public int AttackIncreaseAmount;
    public int HealthIncreaseAmount;
    public IncreaseStatsGA(int attackIncreaseAmount, int healthIncreaseAmount)
    {
        AttackIncreaseAmount = attackIncreaseAmount;
        HealthIncreaseAmount = healthIncreaseAmount;
    }

    /*private void IncreaseStatsPerformer(IncreaseStatsGA increaseStatsGA)
    {
        int attackIncreaseAmount = increaseStatsGA.AttackIncreaseAmount;
        int healthIncreaseAmount = increaseStatsGA.HealthIncreaseAmount;

        this.attack += attackIncreaseAmount;
        this.health += healthIncreaseAmount;
    }*/
}
