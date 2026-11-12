using SerializeReferenceEditor;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Tutorial")]
public class TutorialData : ScriptableObject
{
    [field: SerializeField] public EnemyWave Wave { get; private set; }
    [field: SerializeField] public int HeroHealth { get; private set; } = 100;
    [field: SerializeField] public List<CardData> FixedHand { get; private set; } // drawn in this order
    [field: SerializeField] public List<PerkData> Perks { get; private set; }     // usually empty
    [field: SerializeReference, SR] public List<TutorialStep> Steps { get; private set; }
}

[System.Serializable]
public class TutorialStep
{
    [field: SerializeField, TextArea] public string Message { get; private set; }
    [field: SerializeField] public CardData AllowedCard { get; private set; }   // null = no card can be touched
    [field: SerializeField] public bool AllowEndTurn { get; private set; }
    [field: SerializeField] public bool WaitForClick { get; private set; }     // "Click to continue" steps
    [field: SerializeReference, SR] public TutorialCondition Completion { get; private set; }
    [field:SerializeField] public TutorialTarget Target { get; private set; } = TutorialTarget.None;
}

[System.Serializable]
public abstract class TutorialCondition
{
    public abstract void Subscribe(System.Action onMet);
    public abstract void Unsubscribe();
}