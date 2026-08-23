using DG.Tweening;
using UnityEngine;

public class CardViewCreator : MonoBehaviour
{
    [SerializeField] private CardView cardViewPrefab;

    private void Awake() { ServiceLocator.Register<CardViewCreator>(this); }

    public CardView CreateCardView(Card card, Vector3 position, Quaternion rotation)
    {
        CardView cv = Instantiate(cardViewPrefab, position, rotation);
        cv.transform.localScale = Vector3.one;
        cv.transform.DOScale(Vector3.one, 0.15f);

        cv.Setup(card);

        return cv;
    }
}
