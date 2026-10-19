using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum RoomType
{
    BATTLE,
    CAMPSITE,
    SHOP,
    BOSS
}

// Plain C# class (not a MonoBehaviour): the map is generated once per run and stored in MatchData
public class MapNode
{
    public string Id { get; }
    public int Floor { get; }
    public int Column { get; }
    public RoomType Type { get; set; }
    public bool TypeAssigned { get; set; }
    public Vector2 Position { get; set; }

    // paths: Next = rooms on the following floor, Previous = rooms on the previous floor
    public List<MapNode> Next { get; } = new();
    public List<MapNode> Previous { get; } = new();

    // room content (decided at generation time so it survives scene changes)
    public EnemyWave Wave { get; set; }
    public List<CardData> Rewards { get; } = new();

    public MapNode(int floor, int column)
    {
        Floor = floor;
        Column = column;
        Id = $"node_{floor}_{column}";
    }
}

public class MapData
{
    public int Seed { get; set; }

    // Floors[0] = first floor ... Floors[last] = boss floor (single node)
    public List<List<MapNode>> Floors { get; } = new();

    public MapNode Boss => Floors[Floors.Count - 1][0];
    public IEnumerable<MapNode> AllNodes() => Floors.SelectMany(f => f);
    public MapNode GetNode(string id) => AllNodes().FirstOrDefault(n => n.Id == id);
}