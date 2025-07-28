using Cysharp.Threading.Tasks;
using RPG_003.Battle;
using RPG_003.Skills;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPG_003.StatesEffect
{
    [Serializable]
    public class DOT : TimeBaseEffect
    {
        // フォーマット
        // 追加時の残りターン: ダメージデータのリスト(DamageDataにのっとる)
        [SerializeField] private List<DamageData> _damageData = new();
        [SerializeField] private string _onAddedMessage = string.Empty;
        public override string OnAddedMessage => _onAddedMessage;

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
    }
}