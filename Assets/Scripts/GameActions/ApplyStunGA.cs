using UnityEngine;

public class ApplyStunGA : GameAction
{
    public int StunStacks { get; private set; }
    public CombatantView Target { get; private set; }
    public ApplyStunGA(int stunStacks, CombatantView target)
    {
        StunStacks = stunStacks;
        Target = target;
    }
}
