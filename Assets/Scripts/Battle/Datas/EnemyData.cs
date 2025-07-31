using UnityEngine;
using RPG_003.DataManagements.Datas;
using System;
using UnityEngine.AddressableAssets;
using Cysharp.Threading.Tasks;

namespace RPG_003.Battle
{
    [Serializable]
    [CreateAssetMenu(fileName = "EnemyData", menuName = "RPG_003/Enemy/EnemyData", order = 1)]
    public class EnemyData : ScriptableObject, INeedLoad
    {
        public string id;
        public string enemyName;
        public string description;
        public string iconPath = string.Empty;
        public StatusData statusData;
        public string skillBehaviour;
        public string skills; // idをSkillBehaviourごとに決まった形式で保存する
        public Sprite icon;
        private ISkillBehaviour _cache;
        public ISkillBehaviour Behaviour
        {
            get
            {
                if (_cache == null)
                {
                    _cache = ISkillBehaviour.Create(skillBehaviour);
                    _cache?.Initialize();
                }
                return _cache;
            }
        }
        public async UniTask LoadData()
        {
            if (string.IsNullOrEmpty(iconPath) || icon != null) return;
            icon = await Addressables.LoadAssetAsync<Sprite>(iconPath).ToUniTask();
        }

        private void Awake()
        {
            statusData.Name = enemyName;
        }
    }
}