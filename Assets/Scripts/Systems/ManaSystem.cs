using System.Collections;
using System.Reflection.Metadata.Ecma335;
using UnityEngine;

public class ManaSystem : Singleton<ManaSystem>
{
    [SerializeField] private ManaUI manaUI;
    private int maxMana;
    private int currentMana;
    private void OnEnable()
    {
        ActionSystem.AttachPerformer<SpendManaGA>(SpendManaPerformer);
        ActionSystem.AttachPerformer<RefillManaGA>(RefillManaPerformer);
        ActionSystem.AttachPerformer<IncreaseMaxManaGA>(IncreaseMaxManaPerformer);
    }
    private void OnDisable()
    {
        ActionSystem.DetachPerformer<SpendManaGA>();
        ActionSystem.DetachPerformer<RefillManaGA>();
        ActionSystem.DetachPerformer<IncreaseMaxManaGA>();
    }

    public void Setup(int heroMaxMana)
    {
        maxMana = heroMaxMana;
        currentMana = maxMana;
        manaUI.UpdateManaText(currentMana);
    }

    public bool HasEnoughMana(int manaCost) { return currentMana >= manaCost; }
    private IEnumerator SpendManaPerformer(SpendManaGA spendManaGA)
    {
        currentMana -= spendManaGA.Amount;
        manaUI.UpdateManaText(currentMana);
        yield return null;
    }
    private IEnumerator RefillManaPerformer(RefillManaGA refillManaGA)
    {
        currentMana = maxMana;
        manaUI.UpdateManaText(currentMana);
        yield return null;
    }
    private IEnumerator IncreaseMaxManaPerformer(IncreaseMaxManaGA ga)
    {
        maxMana += ga.Amount;
        currentMana += ga.Amount;
        manaUI.UpdateManaText(currentMana);
        yield return null;
    }
}
