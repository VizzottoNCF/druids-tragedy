using UnityEngine;

public class Interactions : Singleton<Interactions>
{
    public bool PlayerIsDragging { get; set; } = false;
    public bool PlayerCanInteract()
    {
        return !ActionSystem.Instance.IsPerforming && !ActionSystem.Instance.IsHalted;
    }
    public bool PlayerCanHover()
    {
        if (PlayerIsDragging) return false;
        return true;
    }
}
