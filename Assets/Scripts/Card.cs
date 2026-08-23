using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Card
{
    public string Title => data.Title;
    public string Description => data.Description;
    public Sprite Image => data.Image;
    public Effect ManualEffect => data.ManualTargetEffect;
    public List<AutoTargetEffect> OtherEffects => data.OtherEffects;

    public int Sequence;

    private readonly CardData data;
    public Card(CardData cardData) 
    { 
        data = cardData; 
        Sequence = cardData.Sequence;
    }
}
