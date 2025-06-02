// Assets/Editor/PositioningEditor.cs
#if UNITY_EDITOR
using RPG_001.Battle;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Positioning))]
public class PositioningEditor : Editor
{
    // 可視化するときに呼ばれる
    private void OnSceneGUI()
    {
        var positioning = (Positioning)target;
        // シリアライズされた entries リストを取得
        serializedObject.Update();
        var entriesProp = serializedObject.FindProperty("_entries");

        for (int i = 0; i < entriesProp.arraySize; i++)
        {
            var entry = entriesProp.GetArrayElementAtIndex(i);
            var coordProp = entry.FindPropertyRelative("coordinates");
            var posEnumProp = entry.FindPropertyRelative("position");

            // Vector2 → Scene上の Vector3 に変換
            Vector2 coord2D = coordProp.vector2Value;
            Vector3 worldPos = new Vector3(coord2D.x, coord2D.y, 0f);

            // ハンドルを描画＆操作
            EditorGUI.BeginChangeCheck();
            Vector3 newWorldPos = Handles.PositionHandle(
                worldPos,
                Quaternion.identity
            );
            // ラベルも出しとく
            string label = posEnumProp.enumDisplayNames[posEnumProp.enumValueIndex];
            Handles.Label(worldPos + Vector3.up * 0.3f, label);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(positioning, "Move PositionEntry Handle");
                // 更新した座標を Vector2 に戻してプロパティにセット
                coordProp.vector2Value = new Vector2(newWorldPos.x, newWorldPos.y);
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif
