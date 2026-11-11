using System;
using UnityEngine;

// Prefab needs: SpriteRenderer + Collider2D (OnMouseDown works the same way your old Encounter did)
[RequireComponent(typeof(SpriteRenderer))]
public class MapNodeView : MonoBehaviour
{
    [SerializeField] private Color visitedColor = new(0.4f, 0.4f, 0.4f, 1f);
    [SerializeField] private Color availableColor = Color.white;
    [SerializeField] private Color lockedColor = new(1f, 1f, 1f, 0.35f);

    public MapNode Node { get; private set; }
    public bool Available { get; private set; }

    private SpriteRenderer sr;
    private Action<MapNodeView> onClick;
    private string nodeDescription;
    private string nodeType;

    public void Setup(MapNode node, Sprite sprite, Action<MapNodeView> clicked, string type, string description)
    {
        sr = GetComponent<SpriteRenderer>();
        Node = node;
        onClick = clicked;
        sr.sprite = sprite;
        gameObject.name = node.Id;
        nodeType = type;
        nodeDescription = description;
    }

    public void SetState(bool visited, bool available)
    {
        Available = available;
        sr.color = visited ? visitedColor : available ? availableColor : lockedColor;
    }

    private void OnMouseDown()
    {
        // ignore clicks while the deck edit screen is open
        if (DeckEditSystem.Instance != null && DeckEditSystem.Instance.IsEditing) return;
        if (!Available) return;

        onClick?.Invoke(this);
    }

    private void OnMouseEnter()
    {
        FindAnyObjectByType<NodeInfoUI>()?.ShowNodeInfo(nodeType, nodeDescription);
    }
    private void OnMouseExit()
    {
        FindAnyObjectByType<NodeInfoUI>()?.HideNodeInfo();
    }
}