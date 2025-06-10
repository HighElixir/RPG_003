using RPG_003.Battle.Characters;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RPG_003.Battle
{
    // シーンビューでハンドルを表示するために ExecuteInEditMode を追加
    [ExecuteInEditMode]
    public class CharacterTransformHalper : SerializedMonoBehaviour
    {
        // インスペクタに表示できる辞書（Odin がシリアライズをサポートしてる）
        [SerializeField]
        private Dictionary<CharacterPosition, Vector2> _entries = new();

        /// <summary>
        /// 指定されたポジションの座標を取得してオブジェクトを移動する
        /// </summary>
        public void SetPosition(CharacterBase target, CharacterPosition position)
        {
            if (!_entries.TryGetValue(position, out var p))
            {
                Debug.LogError($"Position {position} の座標が登録されてないよ！entriesをチェックしてね");
                return;
            }

            target.transform.position = new Vector3(p.x, p.y, target.transform.position.z);
        }

        public Vector2 GetPosition(CharacterPosition position)
        {
            return _entries.TryGetValue(position, out var p) ? p : Vector2.zero;
        }
#if UNITY_EDITOR
        // シーンビューにハンドルを表示して動かせるようにする
        private void OnDrawGizmos()
        {
            if (_entries == null || _entries.Count == 0)
                return;

            // Undo のためにオブジェクトをキャッシュ
            var serializedObject = new SerializedObject(this);
            serializedObject.Update();

            // Dictionary のキーをリスト化してループ（同時にキーの変更を避けるため）
            foreach (var kv in _entries.ToList())
            {
                var key = kv.Key;
                var vec2 = kv.Value;
                // Vector2 -> Vector3（Z座標を 0 に固定）
                var handlePos = new Vector3(vec2.x, vec2.y, 0f);

                // ハンドルを描画（回転は Quaternion.identity、録画用に Undo を使う）
                EditorGUI.BeginChangeCheck();
                var newHandlePos = Handles.PositionHandle(handlePos, Quaternion.identity);
                if (EditorGUI.EndChangeCheck())
                {
                    // 動かした瞬間に Undo を登録して、元に戻せるようにする
                    Undo.RecordObject(this, $"Move Position for {key}");
                    // 辞書の値を更新（Vector3→Vector2 に戻す）
                    _entries[key] = new Vector2(newHandlePos.x, newHandlePos.y);
                    // シーンのシリアライズ情報を「変更あり」にマーク
                    EditorUtility.SetDirty(this);
                }

                // ラベル表示（何のポジションか分かるように）
                Handles.Label(handlePos + Vector3.up * 0.5f, key.ToString());
            }

            serializedObject.ApplyModifiedProperties();
        }
#endif
    }
}
