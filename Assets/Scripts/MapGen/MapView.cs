using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Lives in the "Pathway" scene. Replaces the hand placed Encounter objects.
public class MapView : MonoBehaviour
{
    [SerializeField] private MapConfig config;
    [SerializeField] private MapNodeView nodeViewPrefab;

    [Header("Room Sprites")]
    [SerializeField] private Sprite battleSprite;
    [SerializeField] private Sprite campsiteSprite;
    [SerializeField] private Sprite shopSprite;
    [SerializeField] private Sprite bossSprite;

    [Header("Lines")]
    [SerializeField] private Material lineMaterial; // optional, falls back to Sprites/Default
    [SerializeField] private float lineWidth = 0.08f;
    [SerializeField] private Color travelledLineColor = Color.white;
    [SerializeField] private Color idleLineColor = new(1f, 1f, 1f, 0.3f);

    [Header("UI")]
    public GameObject shopUI;
    [SerializeField] private PerkOption perkOptionPrefab;

    private readonly List<PerkOption> shopOptions = new();
    private readonly Dictionary<string, MapNodeView> views = new();
    private readonly List<(MapNode from, MapNode to, LineRenderer lr)> lines = new();
    private Transform linesRoot;

    private IEnumerator Start()
    {
        // MatchData lives in the Bootstrap scene, wait until it exists and ran its Start()
        yield return new WaitUntil(() => MatchData.Instance != null && MatchData.Instance.CompletedEncounters != null);

        MatchData match = MatchData.Instance;

        // generate only once per run, the map must survive Pathway <-> Combat scene changes
        if (match.Map == null)
        {
            int seed = config.UseFixedSeed ? config.Seed : Random.Range(0, int.MaxValue);
            match.Map = MapGenerator.Generate(config, seed);
            Debug.Log($"Map generated with seed {seed}");
        }

        Build(match.Map);
        Refresh();
        TryOfferRewards(match);
    }

    private void Build(MapData map)
    {
        linesRoot = new GameObject("Lines").transform;
        linesRoot.SetParent(transform, false);

        foreach (MapNode node in map.AllNodes())
        {
            foreach (MapNode next in node.Next) CreateLine(node, next);
        }

        foreach (MapNode node in map.AllNodes())
        {
            MapNodeView view = Instantiate(nodeViewPrefab, transform);
            view.transform.localPosition = node.Position;
            view.Setup(node, GetSprite(node.Type), OnNodeClicked);
            views.Add(node.Id, view);
        }
    }

    private void CreateLine(MapNode from, MapNode to)
    {
        var go = new GameObject($"Line_{from.Id}_{to.Id}");
        go.transform.SetParent(linesRoot, false);

        var lr = go.AddComponent<LineRenderer>();
        lr.useWorldSpace = false;
        lr.positionCount = 2;
        lr.SetPosition(0, from.Position);
        lr.SetPosition(1, to.Position);
        lr.startWidth = lr.endWidth = lineWidth;
        lr.sortingOrder = -1;
        lr.material = lineMaterial != null ? lineMaterial : new Material(Shader.Find("Sprites/Default"));

        lines.Add((from, to, lr));
    }

    private void Refresh()
    {
        MatchData match = MatchData.Instance;
        MapNode current = string.IsNullOrEmpty(match.CurrentNodeID) ? null : match.Map.GetNode(match.CurrentNodeID);

        foreach (MapNodeView view in views.Values)
        {
            bool visited = match.CompletedEncounters.Contains(view.Node.Id);
            bool available = !visited && IsReachable(match, current, view.Node);
            view.SetState(visited, available);
        }

        foreach (var line in lines)
        {
            bool travelled = match.CompletedEncounters.Contains(line.from.Id) && match.CompletedEncounters.Contains(line.to.Id);
            line.lr.startColor = line.lr.endColor = travelled ? travelledLineColor : idleLineColor;
        }
    }

    // first floor is open at the start; after that only the children of the node you just finished
    private static bool IsReachable(MatchData match, MapNode current, MapNode node)
    {
        if (current == null) return node.Floor == 0;
        return match.CompletedEncounters.Contains(current.Id) && current.Next.Contains(node);
    }

    private void OnNodeClicked(MapNodeView view)
    {
        if (IsShopOpen) return;

        MapNode node = view.Node;
        MatchData match = MatchData.Instance;

        switch (node.Type)
        {
            case RoomType.BATTLE:
            case RoomType.BOSS:
                if (node.Wave == null) { Debug.LogError($"{node.Id} has no EnemyWave. Check the lists in MapConfig"); return; }

                match.CurrentNodeID = node.Id;
                match.PickNewEncounter(node.Wave);

                // node.Id is the encounterID, so EndMatchGA keeps marking it in CompletedEncounters
                ActionSystem.Instance.Perform(new StartMatchGA(node.Id));
                break;

            case RoomType.CAMPSITE:
                MatchData.Instance.HeroRemainingHealth += 20;
                CompleteInstantly(node);
                break;

            case RoomType.SHOP:
                OpenShop(node);
                break;
        }
    }

    // rooms with no combat scene are finished right away (replace this when their UIs exist)
    public void CompleteInstantly(MapNode node)
    {
        MatchData.Instance.CurrentNodeID = node.Id; // set here, so leaving a shop never softlocks
        MatchData.Instance.CompletedEncounters.Add(node.Id);
        Refresh();
    }

    public bool IsShopOpen => shopUI != null && shopUI.activeSelf;

    private void OpenShop(MapNode node)
    {
        shopUI.SetActive(true);
        ClearShopOptions();

        foreach (PerkData perk in node.ShopPerks)
        {
            PerkOption option = Instantiate(perkOptionPrefab, shopUI.transform);
            option.Setup(perk, node, this);
            shopOptions.Add(option);
        }
    }

    // called by PerkOption: only one can ever be taken because the shop closes right after
    public void ChooseShopPerk(PerkData perk, MapNode node)
    {
        if (!IsShopOpen) return;

        MatchData.Instance.Perks.Add(perk);
        CloseShop();
        CompleteInstantly(node);
    }

    // hook this to a "Leave" button so the player can back out without buying
    public void CloseShop()
    {
        ClearShopOptions();
        shopUI.SetActive(false);
    }

    private void ClearShopOptions()
    {
        foreach (PerkOption o in shopOptions) if (o != null) Destroy(o.gameObject);
        shopOptions.Clear();
    }

    // same behaviour as the old Encounter.Complete: back from a battle -> offer the deck edit once
    private void TryOfferRewards(MatchData match)
    {
        if (string.IsNullOrEmpty(match.CurrentNodeID)) return;

        MapNode current = match.Map.GetNode(match.CurrentNodeID);
        if (current == null || !match.CompletedEncounters.Contains(current.Id)) return;

        if (current.Type == RoomType.BOSS)
        {
            Debug.Log("Boss defeated: TODO end of run / next act");
            return;
        }

        if (current.Rewards.Count > 0 && match.RewardsClaimed.Add(current.Id))
        {
            DeckEditSystem.Instance.StartEdit(current.Rewards);
        }
    }

    private Sprite GetSprite(RoomType type)
    {
        return type switch
        {
            RoomType.BATTLE => battleSprite,
            RoomType.CAMPSITE => campsiteSprite,
            RoomType.SHOP => shopSprite,
            RoomType.BOSS => bossSprite,
            _ => null
        };
    }
}