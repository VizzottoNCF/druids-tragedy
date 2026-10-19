using DG.Tweening;
using System.Collections.Generic;
using Unity.AI.Navigation.LowLevel;
using UnityEditor.U2D.Aseprite;
using UnityEngine;

public static class MapGenerator
{
    public static MapData Generate(MapConfig cfg, int seed)
    {
        // seed system so it has predetermined map generation
        var rng = new System.Random(seed);
        var map = new MapData { Seed = seed };

        MapNode[,] grid = CreatePaths(cfg, rng);
        BuildFloors(cfg, map, grid);
        AddBoss(cfg, map);
        AssignRoomTypes(cfg, rng, map);
        AssignContent(cfg, rng, map);
        ApplyLayout(cfg, rng, map);
        return map;
    }

    // ---------------------------------------------------------------- 1) paths
    private static MapNode[,] CreatePaths(MapConfig cfg, System.Random rng)
    {
        var grid = new MapNode[cfg.Floors, cfg.Columns];
        int firstStart = -1;

        for (int p = 0; p < cfg.PathCount; p++)
        {
            int col = rng.Next(cfg.Columns);

            // the first 2 paths must start in different columns (at least 2 starting rooms)
            if (p == 1) { while (col == firstStart) col = rng.Next(cfg.Columns); }
            if (p == 0) firstStart = col;

            MapNode current = GetOrCreate(grid, 0, col);
            for (int f = 0; f < cfg.Floors - 1; f++)
            {
                int nextCol = PickNextColumn(cfg, rng, grid, f, current.Column);
                MapNode next = GetOrCreate(grid, f + 1, nextCol);
                Link(current, next);
                current = next;
            }
        }
        return grid;
    }

    private static int PickNextColumn(MapConfig cfg, System.Random rng, MapNode[,] grid, int floor, int col)
    {
        // can go to the closest column on the left, straight up, or the closest on the right
        var options = new List<int>();
        for (int d = -1; d <= 1; d++)
        {
            int c = col + d;
            if (c < 0 || c >= cfg.Columns) continue;
            if (CrossesExistingPath(grid, floor, col, c)) continue;
            options.Add(c);
        }
        return options[rng.Next(options.Count)]; // going straight up never crosses, so never empty
    }

    private static bool CrossesExistingPath(MapNode[,] grid, int floor, int from, int to)
    {
        if (from == to) return false;

        // neighbour on the same floor, on the side we're moving to
        MapNode neighbour = grid[floor, to];
        if (neighbour == null) return false;

        // if it already goes to our column on the next floor, the two lines would form an X
        return neighbour.Next.Exists(n => n.Column == from);
    }

    private static MapNode GetOrCreate(MapNode[,] grid, int floor, int col)
    {
        grid[floor, col] ??= new MapNode(floor, col);
        return grid[floor, col];
    }

    private static void Link(MapNode from, MapNode to)
    {
        if (from.Next.Contains(to)) return;
        from.Next.Add(to);
        to.Previous.Add(from);
    }

    // ---------------------------------------------------------------- 2) floors + boss
    private static void BuildFloors(MapConfig cfg, MapData map, MapNode[,] grid)
    {
        // only nodes touched by a path exist in the grid, "pathless" rooms are never created
        for (int f = 0; f < cfg.Floors; f++)
        {
            var floor = new List<MapNode>();
            for (int c = 0; c < cfg.Columns; c++)
            {
                if (grid[f, c] != null) floor.Add(grid[f, c]);
            }
            map.Floors.Add(floor);
        }
    }

    private static void AddBoss(MapConfig cfg, MapData map)
    {
        var boss = new MapNode(cfg.Floors, cfg.Columns / 2) { Type = RoomType.BOSS, TypeAssigned = true };
        foreach (MapNode node in map.Floors[cfg.Floors - 1]) Link(node, boss);
        map.Floors.Add(new List<MapNode> { boss });
    }

    // ---------------------------------------------------------------- 3) room types
    private static void AssignRoomTypes(MapConfig cfg, System.Random rng, MapData map)
    {
        // bottom -> top, so parents are always assigned before their children
        for (int f = 0; f < cfg.Floors; f++)
        {
            foreach (MapNode node in map.Floors[f])
            {
                node.Type = RollType(cfg, rng, node);
                node.TypeAssigned = true;
            }
        }
    }

    private static RoomType RollType(MapConfig cfg, System.Random rng, MapNode node)
    {
        if (node.Floor == 0) return RoomType.BATTLE;
        if (node.Floor == cfg.Floors - 1 && cfg.LastFloorIsCampsite) return RoomType.CAMPSITE;

        // hard rules: floor limits and "no campsite -> campsite / shop -> shop"
        var allowed = cfg.Weights.FindAll(w => w.Weight > 0 && w.Type != RoomType.BOSS && IsAllowed(cfg, node, w.Type));
        if (allowed.Count == 0) return RoomType.BATTLE;

        // soft rule: siblings (rooms sharing a parent) should have different types.
        // Soft because with only 3 types it can be impossible (e.g. early floors only allow BATTLE)
        var unique = allowed.FindAll(w => !SiblingHasType(node, w.Type));
        return WeightedPick(rng, unique.Count > 0 ? unique : allowed);
    }

    private static bool IsAllowed(MapConfig cfg, MapNode node, RoomType type)
    {
        switch (type)
        {
            case RoomType.CAMPSITE:
                if (node.Floor < cfg.MinCampsiteFloor) return false;
                // never right before the final campsite floor
                if (cfg.LastFloorIsCampsite && node.Floor == cfg.Floors - 2) return false;
                return !ParentHasType(node, type);
            case RoomType.SHOP:
                if (node.Floor < cfg.MinShopFloor) return false;
                return !ParentHasType(node, type);
            default:
                return true; // BATTLE can repeat
        }
    }

    private static bool ParentHasType(MapNode node, RoomType type)
    {
        return node.Previous.Exists(p => p.TypeAssigned && p.Type == type);
    }

    private static bool SiblingHasType(MapNode node, RoomType type)
    {
        foreach (MapNode parent in node.Previous)
        {
            foreach (MapNode sibling in parent.Next)
            {
                if (sibling != node && sibling.TypeAssigned && sibling.Type == type) return true;
            }
        }
        return false;
    }

    private static RoomType WeightedPick(System.Random rng, List<RoomWeight> pool)
    {
        float total = 0f;
        foreach (RoomWeight w in pool) total += w.Weight;

        double roll = rng.NextDouble() * total;
        foreach (RoomWeight w in pool)
        {
            roll -= w.Weight;
            if (roll <= 0) return w.Type;
        }
        return pool[pool.Count - 1].Type;
    }

    // ---------------------------------------------------------------- 4) content (waves / rewards)
    private static void AssignContent(MapConfig cfg, System.Random rng, MapData map)
    {
        var bag = new List<RewardData>();

        foreach (MapNode node in map.AllNodes())
        {
            switch (node.Type)
            {
                case RoomType.BATTLE:
                    node.Wave = PickRandom(cfg.BattleWaves, rng);

                    RewardData reward = DrawFromBag(cfg.RewardSets, bag, rng);
                    if (reward != null) node.Rewards.AddRange(reward.Cards);
                    break;
                case RoomType.BOSS:
                    node.Wave = PickRandom(cfg.BossWaves, rng);
                    break;
            }
        }
    }

    // shuffle-bag: every reward is used once before any of them repeats
    private static RewardData DrawFromBag(List<RewardData> source, List<RewardData> bag, System.Random rng)
    {
        if (source == null || source.Count == 0)
        {
            Debug.LogWarning("MapGenerator: RewardSets is empty in MapConfig");
            return null;
        }

        if (bag.Count == 0) bag.AddRange(source);

        int idx = rng.Next(bag.Count);
        RewardData picked = bag[idx];
        bag.RemoveAt(idx);
        return picked;
    }

    private static T PickRandom<T>(List<T> list, System.Random rng)
    {
        if (list == null || list.Count == 0)
        {
            Debug.LogWarning($"MapGenerator: empty list of {typeof(T).Name} in MapConfig");
            return default;
        }
        return list[rng.Next(list.Count)];
    }

    // ---------------------------------------------------------------- 5) layout
    private static void ApplyLayout(MapConfig cfg, System.Random rng, MapData map)
    {
        float Jitter() => ((float)rng.NextDouble() - 0.5f) * 2f * cfg.Jitter;

        foreach (var floor in map.Floors)
        {
            foreach (MapNode node in floor)
            {
                float along = (node.Floor - cfg.Floors / 2f) * cfg.FloorSpacing;
                float across = (node.Column - (cfg.Columns - 1) / 2f) * cfg.ColumnSpacing;

                if (node.Type == RoomType.BOSS) across = 0f;
                else { along += Jitter(); across += Jitter(); }

                node.Position = cfg.Horizontal ? new Vector2(along, across) : new Vector2(across, along);
            }
        }
    }
}