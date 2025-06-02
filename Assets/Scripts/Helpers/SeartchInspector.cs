using RPG_001.Battle.Characters.Enemy;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static RPG_001.Battle.Characters.Enemy.EnemyBehaviorData;

namespace RPG_001
{
    public class EnemyBehaviorDataSearchWindow : OdinMenuEditorWindow
    {
        [Searchable]                  // キーワード検索バーを自動生成
        public string keyword;

        [ValueDropdown("GetAllTypes"), LabelText("ActionType絞り込み")]
        public ActionMapType? typeFilter;

        [LabelText("重みの範囲")]
        [MinMaxSlider(0f, 10f)]
        public float[] weightRange = new float[] { 0f, 10f };

        [ValueDropdown("GetAllSkills"), LabelText("スキル絞り込み")]
        public EnemySkill skillFilter;

        protected override OdinMenuTree BuildMenuTree()
        {
            titleContent = new GUIContent("EnemyBehaviorData 検索");
            var tree = new OdinMenuTree();
            tree.Config.DrawSearchToolbar = false;   // 独自検索フィールドを使う

            var guids = AssetDatabase.FindAssets("t:EnemyBehaviorData");
            foreach (var guid in guids)
            {
                var asset = AssetDatabase.LoadAssetAtPath<EnemyBehaviorData>(
                    AssetDatabase.GUIDToAssetPath(guid)
                );

                if (Matches(asset))
                    tree.Add(asset.name, asset);
            }
            return tree;
        }

        private bool Matches(EnemyBehaviorData asset)
        {
            var q = keyword?.ToLower().Trim();
            foreach (var kv in asset.enemyActions)
            {
                // １．ActionMapType フィルタ
                if (typeFilter != null && kv.Key != typeFilter) continue;

                // ２．weight 範囲フィルタ
                if (kv.Value.weight < weightRange[0] || kv.Value.weight > weightRange[1]) continue;

                // ３．スキルフィルタ
                //if (skillFilter != null && !kv.Value.usableSkills.Contains(skillFilter)) continue;

                // ４．キーワードフィルタ（キー名／weight／スキル名にマッチ）
                if (!string.IsNullOrEmpty(q))
                {
                    if (kv.Key.ToString().ToLower().Contains(q)
                     || kv.Value.weight.ToString().Contains(q)
                     || kv.Value.usableSkills.Any(s => s.ToString().ToLower().Contains(q)))
                    {
                        return true;
                    }
                    else continue;
                }

                // フィルタ無しならマッチ
                return true;
            }
            return false;
        }

        private IEnumerable<ActionMapType> GetAllTypes()
            => System.Enum.GetValues(typeof(ActionMapType)).Cast<ActionMapType>();

        private IEnumerable<EnemySkill> GetAllSkills()
            => System.Enum.GetValues(typeof(EnemySkill)).Cast<EnemySkill>();

        [MenuItem("Tools/Enemy/EnemyBehaviorData 検索")]
        private static void Open() => GetWindow<EnemyBehaviorDataSearchWindow>().Show();
    }
}