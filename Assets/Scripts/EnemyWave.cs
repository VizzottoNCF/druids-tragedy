using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "EnemyWave")]
public class EnemyWave : ScriptableObject
{
    public List<EnemyData> enemyDatas;
}
