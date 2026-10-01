using UnityEngine;

public class HeroView : CombatantView
{
    public void Setup(HeroData heroData)
    {
        Debug.Log($"Setting up hero view with health: {heroData.Health}, remaining health: {MatchData.Instance.HeroRemainingHealth}");
        SetupBase(heroData.Health, MatchData.Instance.HeroRemainingHealth, heroData.Image);
    }
}
