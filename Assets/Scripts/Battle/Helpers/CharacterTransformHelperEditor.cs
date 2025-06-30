#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace RPG_003.Battle
{
    [CustomEditor(typeof(CharacterTransformHelper))]
    public class CharacterTransformHelperEditor : OdinEditor
    {
        private CharacterTransformHelper helper;

        protected override void OnEnable()
        {
            helper = target as CharacterTransformHelper;
        }

        private void OnSceneGUI()
        {
            // 辞書をループするときはコピーしておくと安全
            foreach (var kv in helper._entries.ToList())
            {
                var key = kv.Key;
                var vec = kv.Value;
                var worldPos = new Vector3(vec.x, vec.y, 0f);

                EditorGUI.BeginChangeCheck();
                var newWorldPos = Handles.PositionHandle(worldPos, Quaternion.identity);

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(helper, $"Move Position {key}");
                    // 変更後の座標で更新
                    helper._entries[key] = new Vector2(newWorldPos.x, newWorldPos.y);
                    EditorUtility.SetDirty(helper);
                }

                Handles.Label(worldPos + Vector3.up * 0.5f, key.ToString());
            }
        }
    }
}
#endif
