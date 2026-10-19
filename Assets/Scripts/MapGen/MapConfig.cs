using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RoomWeight
{
    public RoomType Type;
    public float Weight = 1f;
}

[CreateAssetMenu(menuName = "Data/Map Config")]
public class MapConfig : ScriptableObject
{
    [Header("Grid")]
    public int Floors = 10;      // floors before the boss (boss is added on top)
    public int Columns = 5;
    public int PathCount = 4;    // number of random walks from floor 0 to the last floor

    [Header("Room Rules")]
    public List<RoomWeight> Weights = new()
    {
        new RoomWeight { Type = RoomType.BATTLE, Weight = 65f },
        new RoomWeight { Type = RoomType.CAMPSITE, Weight = 20f },
        new RoomWeight { Type = RoomType.SHOP, Weight = 15f },
    };
    public int MinCampsiteFloor = 3;       // campsite can't appear below this floor index
    public int MinShopFloor = 2;           // shop can't appear below this floor index
    public bool LastFloorIsCampsite = true; // last floor before the boss is always a campsite

    [Header("Content")]
    public List<EnemyWave> BattleWaves;
    public List<EnemyWave> BossWaves;
    public List<CardData> RewardPool;
    public int RewardsPerBattle = 5;

    [Header("Layout")]
    public bool Horizontal = true;   // left -> right fits the 16:9 camera better than bottom -> top
    public float FloorSpacing = 2.5f;
    public float ColumnSpacing = 1.8f;
    public float Jitter = 0.3f;

    [Header("Seed")]
    public bool UseFixedSeed = false;
    public int Seed = 12345;

    private void OnValidate()
    {
        Floors = Mathf.Max(3, Floors);
        Columns = Mathf.Max(2, Columns);
        PathCount = Mathf.Max(2, PathCount);
    }
}