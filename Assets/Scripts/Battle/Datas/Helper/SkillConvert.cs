using System.Collections.Generic;
using RPG_003.DataManagements.Holders;
using RPG_003.DataManagements.Datas;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using RPG_003.Effect;

namespace RPG_003.Battle
{
    public static class SkillConverter
    {
        public static List<SkillDataInBattle> ConvertList(this List<SkillHolder> skills)
        {
            var list = new List<SkillDataInBattle>();
            foreach (var item in skills)
            {
                list.Add(item.ConvartData());
            }
            return list;
        }
        public static async UniTask<SkillDataInBattle> ConvertEnemySkill(this EnemySkillData skill)
        {
            var vfx = await Addressables.LoadAssetAsync<SoundVFXData>(skill.vfxPath).ToUniTask();
            return SkillDataInBattle.Create()
                .SetName(skill.name)
                .SetDescription(skill.description)
                .SetDamageDatas(skill.damages)
                .SetTarget(skill.target)
                .SetVFX(vfx);
        }
    }
}