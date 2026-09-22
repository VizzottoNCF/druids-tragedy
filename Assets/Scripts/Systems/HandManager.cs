using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.XR;

public class HandManager : MonoBehaviour
{
    public CardData cd;
    public GameObject SelectedCard;
    //public List<AudioClip> drawSounds;

    [Header("Hand Config")]
    public int MaxHandSize;
    public SplineContainer HandSplineContainer;
    public List<GameObject> handCards = new();

    [Header("Tray Config")]
    public int MaxTraySize;
    public SplineContainer TraySplineContainer;
    public List<GameObject> trayCards = new();

    private void Awake() { ServiceLocator.Register<HandManager>(this); }

    public IEnumerator AddCard(Card card, Vector3 position, quaternion rotation)
    {
        DeselectCard();
        if (handCards.Count >= MaxHandSize) { Debug.LogError("HAND SIZE CAPPED"); yield break; }
        if (card == null) { Debug.LogError("Null card"); yield break; }

        //Debug.Log($"{card.Title} // {card==null}");

        CardViewCreator service = ServiceLocator.Get<CardViewCreator>();

        GameObject g = service.CreateCardView(card, position, rotation).gameObject;
        
        handCards.Add(g);

        // Wait for the card position update to complete
        yield return StartCoroutine(UpdateCardPosition(0.15f));
    }

    public IEnumerator RemoveCard(GameObject card, Vector3 position, quaternion rotation, bool delete = false)
    {
        DeselectCard();
        if (trayCards.Contains(card)) { Debug.LogError("Card in tray, not hand"); yield break; }
        if (!handCards.Contains(card)) { Debug.LogError("CARD NOT IN PLAY"); yield break; }

        handCards.Remove(card);

        card.transform.DOMove(position, 0.15f);
        card.transform.DOLocalRotateQuaternion(rotation, 0.15f);

        DeselectCard();

        // Wait for the card position update to complete
        yield return StartCoroutine(UpdateCardPosition(0.15f));
        

        if (delete) { Destroy(card); }
    }
    public IEnumerator RemoveCardTray(GameObject card)
    {
        DeselectCard();
        if (handCards.Contains(card)) { Debug.LogError("Card in hand, not tray"); yield break; }
        if (!trayCards.Contains(card)) { Debug.LogError("CARD NOT IN PLAY"); yield break; }

        trayCards.Remove(card);

        yield return card.transform.DOScale(Vector3.zero, 0.15f).WaitForCompletion();

        DeselectCard();

        // Wait for the card position update to complete
        yield return StartCoroutine(UpdateTrayCardPosition(0.15f));
        
        Destroy(card);
    }

    public IEnumerator PlayCard(GameObject card)
    {
        if (card == null) { Debug.LogError("CARD IS NULL"); yield break; }
        handCards.Remove(card);

        StartCoroutine(UpdateCardPosition(0.15f));
        
        yield return card.transform.DOScale(Vector3.zero, 0.15f).WaitForCompletion();

        DeselectCard();
    }
    public IEnumerator TrayCard(GameObject card)
    {
        if (card == null) { Debug.LogError("CARD IS NULL"); yield break; }
        handCards.Remove(card);
        trayCards.Add(card);

        Vector3 trayCardScale = new Vector3(0.5f, 0.5f, 1f);
        yield return card.transform.DOScale(trayCardScale, 0.15f).WaitForCompletion();

        StartCoroutine(UpdateCardPosition(0.15f));
        StartCoroutine(UpdateTrayCardPosition(0.15f));

        DeselectCard();
    }
    public IEnumerator UntrayCard(GameObject card)
    {
        Debug.Log($"Untraying card /PRENULL");
        if (card == null) { Debug.LogError("CARD IS NULL"); yield break; }
        Debug.Log($"Untraying card /POSTNULL");
        handCards.Add(card);
        trayCards.Remove(card);

        Vector3 handCardScale = new Vector3(1f, 1f, 1f);
        yield return card.transform.DOScale(handCardScale, 0.15f).WaitForCompletion();


        StartCoroutine(UpdateCardPosition(0.15f));
        StartCoroutine(UpdateTrayCardPosition(0.15f));

        DeselectCard();
    }

    public IEnumerator UpdateCardPosition(float duration)
    {
        if (handCards.Count == 0) { yield break; }
        float cardSpacing = 1f / MaxHandSize; // spline lenght is always 1f

        float firstCardPosition = 0.5f - (handCards.Count - 1) * cardSpacing / 2;
        Spline spline = HandSplineContainer.Spline;

        for (int i = 0; i < handCards.Count; i++)
        {
            float p = firstCardPosition + i * cardSpacing;
            Vector3 splinePosition = spline.EvaluatePosition(p);
            Vector3 forward = spline.EvaluateTangent(p);
            Vector3 up = spline.EvaluateUpVector(p);
            Quaternion rotation = Quaternion.LookRotation(up, Vector3.Cross(up, forward).normalized);
            rotation = new Quaternion(0f, 0f, rotation.z, rotation.w);

            handCards[i].transform.DOMove(splinePosition, duration);
            handCards[i].transform.DOLocalRotateQuaternion(rotation, duration);
        }
        yield return new WaitForSeconds(duration);
    }
    public IEnumerator UpdateTrayCardPosition(float duration)
    {
        if (trayCards.Count == 0) { yield break; }
        float cardSpacing = 1f / MaxTraySize; // spline lenght is always 1f

        float firstCardPosition = 0.5f - (trayCards.Count - 1) * cardSpacing / 2;
        Spline spline = TraySplineContainer.Spline;

        for (int i = 0; i < trayCards.Count; i++)
        {
            float p = firstCardPosition + i * cardSpacing;
            Vector3 splinePosition = spline.EvaluatePosition(p);
            Vector3 forward = spline.EvaluateTangent(p);
            Vector3 up = spline.EvaluateUpVector(p);
            Quaternion rotation = Quaternion.LookRotation(up, Vector3.Cross(up, forward).normalized);
            rotation = new Quaternion(0f, 0f, rotation.z, rotation.w);

            trayCards[i].transform.DOMove(splinePosition, duration);
            trayCards[i].transform.DOLocalRotateQuaternion(rotation, duration);
        }
        yield return new WaitForSeconds(duration);
    }
    public void DeselectCard()
    {
        if (SelectedCard == null) { return; }
        
        SelectedCard = null;
    }
}
