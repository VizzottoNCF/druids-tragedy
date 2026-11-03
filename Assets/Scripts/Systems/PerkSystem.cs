using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PerkSystem : Singleton<PerkSystem>
{
    [SerializeField] private PerksUI perksUI;
    private List<Perk> perks = new();
    public void Setup(List<PerkData> p)
    {
        new List<Perk>(p.Select(perkData => new Perk(perkData))).ForEach(AddPerk);
    }
    public void AddPerk(Perk perk)
    {
        perks.Add(perk);
        perksUI.AddPerkUI(perk);
        perk.OnAdd();
    }
    public void RemovePerk(Perk perk)
    {
        perks.Remove(perk);
        perksUI.RemovePerkUI(perk);
        perk.OnRemove();
    }
    private void OnDisable()
    {
        var perksCopy = perks.ToList();
        foreach (var perk in perksCopy) RemovePerk(perk);
        perks.Clear();
    }
}
