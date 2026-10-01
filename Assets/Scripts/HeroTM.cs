using System.Collections.Generic;

[System.Serializable]
public class HeroTM : TargetMode
{
    public override List<CombatantView> GetTargets()
    {
        List<CombatantView> targets = new() { HeroSystem.Instance.heroView };
        return targets;
    }
}
