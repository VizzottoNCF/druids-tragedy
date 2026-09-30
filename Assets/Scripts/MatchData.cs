using UnityEditor;
using UnityEngine;

public class MatchData : Singleton<MatchData>
{
    public EnemyWave wave;

    public void PickNewEncounter(EnemyWave encounter)
    {
        wave = encounter;
    }
}
