using TMPro;
using UnityEngine;
public class CardView : MonoBehaviour
{
    [SerializeField] private TMP_Text cardName;
    [SerializeField] private TMP_Text cardDescription;
    [SerializeField] private TMP_Text cardSequence;
    [SerializeField] private SpriteRenderer imageSR;

    [SerializeField] private GameObject Wrapper;
    [SerializeField] private LayerMask dropLayer;

    public Card Card { get; private set; }
    private Vector3 dragStartPosition;
    private Quaternion dragStartRotation;

    public void Setup(Card card)
    {
        Card = card;
        cardName.text = Card.Title;
        cardDescription.text = Card.Description;
        cardSequence.text = Card.Sequence.ToString();
        imageSR.sprite = Card.Image;
    }

    public void OnMouseEnter()
    {
        if (!Interactions.Instance.PlayerCanHover()) return;

        Wrapper.SetActive(false);
        Vector3 pos = new(transform.position.x, 0, transform.position.z);
        ServiceLocator.Get<CardViewHoverSystem>().Show(Card, pos);
    }
    public void OnMouseExit()
    {
        if (!Interactions.Instance.PlayerCanHover()) return;

        ServiceLocator.Get<CardViewHoverSystem>().Hide();
        Wrapper.SetActive(true);
    }

    public void OnMouseDown()
    {
        if (!Interactions.Instance.PlayerCanInteract()) return;
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
    private void OnMouseDrag()
    {
        if (!Interactions.Instance.PlayerCanInteract()) return;

        if (Card.ManualEffect != null) return;

        transform.position = MouseUtil.GetMousePositionInWorldSpace(-1);
    }
    private void OnMouseUp()
    {
        if (!Interactions.Instance.PlayerCanInteract()) return;

        if (Card.ManualEffect != null)
        {
            EnemyView target = ManualTargetSystem.Instance.EndTargetting(MouseUtil.GetMousePositionInWorldSpace(-1));
            if (target != null && ManaSystem.Instance.HasEnoughMana(Card.Sequence))
            {
                PlayCardGA playCardGA = new(Card, target);
                ActionSystem.Instance.Perform(playCardGA);
            }
        }
        else
        { 
            if (ManaSystem.Instance.HasEnoughMana(Card.Sequence) && Physics.Raycast(transform.position, Vector3.forward, out RaycastHit hit, 10f, dropLayer))
            {
                PlayCardGA playCardGA = new PlayCardGA(Card);
                ActionSystem.Instance.Perform(playCardGA);
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
