﻿using Sirenix.OdinInspector;
using UnityEngine;

namespace RPG_003.Battle
{
    public class BattleDataToUI : MonoBehaviour
    {
        // === Reference ===
        [BoxGroup("Reference"), SerializeField] private ShowDamage showDamage;

        public void CreateDamageText(Transform target, float damage, bool isCrit = false)
        {
            showDamage.Show(target, damage, isCrit);
        }
    }
}