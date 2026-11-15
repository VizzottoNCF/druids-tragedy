using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;
public class CardView : MonoBehaviour
{
    [SerializeField] private TMP_Text cardName;
    [SerializeField] private TMP_Text cardDescription;
    [SerializeField] private TMP_Text cardSequence;
    [SerializeField] private SpriteRenderer imageSR;
    [SerializeField] private Image image;

    [SerializeField] private GameObject Wrapper;
    [SerializeField] private LayerMask dropLayer;

    public Card Card { get; private set; }
    public bool inTray { get; private set; }
    private Vector3 dragStartPosition;
    private Quaternion dragStartRotation;

    public void Setup(Card card)
    {
        Card = card;
        cardName.text = Card.Title;
        cardDescription.text = Card.Description;
        cardSequence.text = Card.Sequence.ToString();
        if (imageSR != null) { imageSR.sprite = Card.Image; }
        if (image != null) { image.sprite = Card.Image; }
        inTray = false;
    }

    public void OnMouseEnter()
    {
        if (!Interactions.Instance.PlayerCanHover()) return;

        if (!inTray)
        {
            Wrapper.SetActive(false);
            Vector3 pos = new(transform.position.x, 0, transform.position.z);
            ServiceLocator.Get<CardViewHoverSystem>().Show(Card, pos);
        }

    }
    public void OnMouseExit()
    {
        if (!Interactions.Instance.PlayerCanHover()) return;

        if (!inTray)
        {
            ServiceLocator.Get<CardViewHoverSystem>().Hide();
            Wrapper.SetActive(true);
        }
    }

    public void OnMouseDown()
    {
        if (!Interactions.Instance.PlayerCanInteract()) return;
        if (TutorialSystem.Active && !TutorialSystem.Instance.CanUseCard(Card)) return;
        
        if (inTray)
        {
            // take off card from tray and put it back in hand
            StartCoroutine(CardSystem.Instance.RemoveCardFromTray(Card, this.gameObject));
            inTray = false;
        }
        else
        {
            HandManager hm = ServiceLocator.Get<HandManager>();
            hm.SelectedCard = this.gameObject;

            if (Card.ManualEffect != null)
            {
                ManualTargetSystem.Instance.StartTargetting(transform.position);
            }
            else
            {
                Interactions.Instance.PlayerIsDragging = true;
                Wrapper.SetActive(true);
                ServiceLocator.Get<CardViewHoverSystem>().Hide();
                dragStartPosition = transform.position;
                dragStartRotation = transform.rotation;

                transform.rotation = Quaternion.Euler(0, 0, 0);
                transform.position = MouseUtil.GetMousePositionInWorldSpace(-1);
            }
        }
    }
    private void OnMouseDrag()
    {
        if (!Interactions.Instance.PlayerCanInteract()) return;
        if (TutorialSystem.Active && !TutorialSystem.Instance.CanUseCard(Card)) return;

        if (Card.ManualEffect != null) return;

        transform.position = MouseUtil.GetMousePositionInWorldSpace(-1);
    }
    private void OnMouseUp()
    {
        if (!Interactions.Instance.PlayerCanInteract()) return;
        if (inTray) return;
        HandManager service = ServiceLocator.Get<HandManager>();
        if (Card.ManualEffect != null)
        {
            EnemyView target = ManualTargetSystem.Instance.EndTargetting(MouseUtil.GetMousePositionInWorldSpace(-1));
            if (target != null && ManaSystem.Instance.HasEnoughMana(Card.Sequence) && service.trayCards.Count < service.MaxTraySize)
            {
                // swaps cards from hand to tray
                StartCoroutine(CardSystem.Instance.PlaceCardInTray(Card, target, this.gameObject));
                inTray = true;

                // play card script is in CardTraySystem, which will be called as a pre reaction to enemy turn
            }
        }
        else
        {
            if (ManaSystem.Instance.HasEnoughMana(Card.Sequence) && Physics.Raycast(transform.position, Vector3.forward, out RaycastHit hit, 10f, dropLayer) && service.trayCards.Count < service.MaxTraySize)
            {
                // swaps cards from hand to tray
                StartCoroutine(CardSystem.Instance.PlaceCardInTray(Card, null, this.gameObject));
                inTray = true;

                // play card script is in CardTraySystem, which will be called as a pre reaction to enemy turn
            }
            else
            {
                transform.position = dragStartPosition;
                transform.rotation = dragStartRotation;
            }
            Interactions.Instance.PlayerIsDragging = false;
        }
    }
}
