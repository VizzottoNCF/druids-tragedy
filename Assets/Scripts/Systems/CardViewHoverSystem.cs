using UnityEngine;

public class CardViewHoverSystem : MonoBehaviour
{
    [SerializeField] private CardView cardViewHover;

    private void Awake() => ServiceLocator.Register<CardViewHoverSystem>(this);

    public void Show(Card card, Vector3 position)
    {
        cardViewHover.gameObject.SetActive(true);
        cardViewHover.Setup(card);
        cardViewHover.transform.position = position;
    }

    public void Hide()
    {
        cardViewHover.gameObject.SetActive(false);
    }
}
