using System;
using System.Collections.Generic;
using UnityEngine;
using RPG_003.DataManagements.Datas;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;

namespace RPG_003.Battle
{
    [Serializable]
    public class PlayerData : INeedLoad
    {
        public StatusData statusData;
        public List<SkillDataInBattle> skillDataInBattles = new List<SkillDataInBattle>();
        public string iconPath;
        public Sprite icon;

        public PlayerData() { }
        public PlayerData(StatusData data, List<SkillDataInBattle> skills)
        {
            statusData = data;
            skillDataInBattles.AddRange(skills);
        }

        public async UniTask LoadData()
        {
            if (string.IsNullOrEmpty(iconPath) || icon != null) return;
            icon = await Addressables.LoadAssetAsync<Sprite>(iconPath).ToUniTask();
        }
    }
}