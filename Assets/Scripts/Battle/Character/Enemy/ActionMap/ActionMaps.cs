using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPG_001.Battle.Characters.Enemy
{
    [CreateAssetMenu(fileName = "ActionMaps", menuName = "RPG_001/Enemy/ActionMaps", order = 1)]
    public class ActionMaps : ScriptableObject
    {
        public List<EnemyAction> enemyActions = new List<EnemyAction>();
    }

    [Serializable]
    public class EnemyAction
    {
        public ActionMapType actionMapType;
        public float weight = 1f; // Default weight for the action
        public List<EnemySkills> usableSkills = new List<EnemySkills>();
    }
    public enum ActionMapType
    {
        None,
        Attack,
        Attack_sub,
        SpecialAttack,
        Special,
        Defend,
        Escape,
    }
}