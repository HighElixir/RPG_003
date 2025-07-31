using Cysharp.Threading.Tasks;
using RPG_003.Battle;
using RPG_003.DataManagements.Datas;
using System;
using System.Collections.Generic;

namespace RPG_003.StatesEffect
{
    [Serializable]
    public class DOT : TimeBaseEffect
    {
        // フォーマット
        // _defaultDuration;"DamageData"
        public List<DamageData> _damageData = new();

        public override async UniTask Update(Unit parent)
        {
            foreach (var data in _damageData)
            {
                var damage = data.MakeDamageInfo(parent, parent);
                // Apply damage to the parent unit
                parent.BattleManager.ApplyDamage(damage);
            }
            await base.Update(parent);
        }

        public override async UniTask Create(string data)
        {
            var values = data.Split(';');
        }
    }
}