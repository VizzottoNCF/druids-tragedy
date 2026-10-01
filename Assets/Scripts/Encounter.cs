using System.Collections.Generic;
using UnityEngine;

public class Encounter : MonoBehaviour
{
    [Header("Settings")]
    private string encounterID;
    public EnemyWave wave;
    public Encounter dependancy = null;
    [Header("Rewards")]
    public List<CardData> Rewards = new List<CardData>();
    [Header("State")]
    public bool available = false;
    public bool complete = false;
    private Collider2D collider;
    private SpriteRenderer sr;
    private LineRenderer lr;

    private void Start()
    {
        encounterID = gameObject.name;
        collider = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();
        lr = GetComponent<LineRenderer>();

        if (MatchData.Instance.CompletedEncounters.Contains(encounterID)) { Complete(encounterID); }
    }

    private void Update()
    {
        CheckAvailability();
    }
    private void CheckAvailability()
    {
        // for encounters without dependancies
        if (dependancy != null)
        {
            // when dependancy has been completed, makes it available
            if (dependancy.complete)
            {
                sr.color = Color.white;

                // check if its already completed, if not, make it available
                if (!complete)
                {
                    available = true;
                }
                else
                {
                    available = false;
                }
            }
        }
        else // if there is no dependancy
        {
            // check if its already completed, if not, make it available
            if (!complete)
            {
                available = true;
            }
            else
            {
                available = false;
            }
        }
    }

    private void OnMouseDown()
    {
        if (DeckEditSystem.Instance != null && DeckEditSystem.Instance.IsEditing) return;

        if (available)
        {
            MatchData.Instance.PickNewEncounter(wave);
            Debug.Log($"{gameObject.name} has been chosen");

            StartMatchGA startMatchGA = new(encounterID);
            ActionSystem.Instance.Perform(startMatchGA);
        }
        else
        {
            Debug.Log($"{gameObject.name} is not available");
        }
    }

    public void Complete(string id)
    {
        if (id != encounterID)
        {
            Debug.Log("id does not match");
            return;
        }
        complete = true;

        // create gradient
        var gradient = new Gradient();
        var colors = new GradientColorKey[1];
        var alphas = new GradientAlphaKey[1];
        colors[0] = new GradientColorKey(Color.white, 1.0f);
        alphas[0] = new GradientAlphaKey(1.0f, 1.0f);
        gradient.SetKeys(colors, alphas);

        lr.colorGradient = gradient;

        // Add() retorna false se já tinha sido oferecido antes
        if (Rewards.Count > 0 && MatchData.Instance.RewardsClaimed.Add(encounterID))
        {
            DeckEditSystem.Instance.StartEdit(Rewards);
        }
    }
}
