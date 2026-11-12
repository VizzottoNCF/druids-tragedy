using UnityEngine;

public class HeroView : CombatantView
{
    public void Setup(HeroData heroData)
    {
        if (TutorialSystem.Active) SetupBase(heroData.Health, TutorialSystem.Instance.data.HeroHealth, heroData.Image);
        else SetupBase(heroData.Health, MatchData.Instance.HeroRemainingHealth, heroData.Image);
    }
}
