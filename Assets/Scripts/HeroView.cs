using UnityEngine;

public class HeroView : CombatantView
{
    public void Setup(HeroData heroData)
    {
        SetupBase(heroData.Health, MatchData.Instance.HeroRemainingHealth, heroData.Image);
    }
}
