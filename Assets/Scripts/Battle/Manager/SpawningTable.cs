using RPG_003.Battle.Characters.Enemy;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPG_003.Battle
{
    [CreateAssetMenu(fileName = "SpawningTable", menuName = "RPG_003/Enemy/SpawningTable", order = 2)]
    public class SpawningTable : SerializedScriptableObject
    {
        [PropertyTooltip("1ウェーブあたりの敵数")]
        public int waveSaize = 25;
        public List<SpawnData> spawnDatas = new List<SpawnData>();

        public SpawnData GetSpawnData(float depth)
        {
            foreach (var spawnData in spawnDatas)
            {
                if (depth >= spawnData.underLimit && depth <= spawnData.overLimit)
                {
                    return spawnData;
                }
            }
            return null; // or handle the case where no matching SpawnData is found
        }
    }

    [Serializable]
    public class SpawnData
    {
        [ValidateInput("@underLimit <= overLimit", "underLimit は overLimit 以下でなきゃダメだよ！")]
        [PropertyTooltip("このスポーンデータが使用される最低の深度")]
        [MinValue(0f)]
        public float underLimit = 0f;
        [PropertyTooltip("このスポーンデータが使用される最大の深度")]
        [MaxValue(1f)]
        public float overLimit = 1f;

        public List<TableData> enemyDataList = new List<TableData>();

        private float _totalWeight = 0f;
        public EnemyData GetEnemyData()
        {
            if (enemyDataList.Count == 0)
                return null;
            // 重み付きランダム選択
            float randomValue = UnityEngine.Random.Range(0f, _totalWeight);
            float cumulativeWeight = 0f;
            foreach (var data in enemyDataList)
            {
                cumulativeWeight += data.weight;
                if (randomValue <= cumulativeWeight)
                {
                    return data.enemyData;
                }
            }
            return null; // ここに到達することはないはず
        }

        private void Awake()
        {
            // 重みの合計を計算
            _totalWeight = 0f;
            foreach (var data in enemyDataList)
            {
                _totalWeight += data.weight;
            }
        }
    }

    [Serializable]
    public class TableData
    {
        [ValidateInput("@weight > 0", "weightは必ず正の値にしてね！")]
        public float weight = 1f;
        public EnemyData enemyData;
    }
}