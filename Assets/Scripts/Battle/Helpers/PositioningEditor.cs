// Assets/Editor/PositioningEditor.cs
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using RPG_001.Battle;

[CustomEditor(typeof(Positioning))]
public class PositioningEditor : Editor
{
    // ��������Ƃ��ɌĂ΂��
    private void OnSceneGUI()
    {
        var positioning = (Positioning)target;
        // �V���A���C�Y���ꂽ entries ���X�g���擾
        serializedObject.Update();
        var entriesProp = serializedObject.FindProperty("entries");

        for (int i = 0; i < entriesProp.arraySize; i++)
        {
            var entry = entriesProp.GetArrayElementAtIndex(i);
            var coordProp = entry.FindPropertyRelative("coordinates");
            var posEnumProp = entry.FindPropertyRelative("position");

            // Vector2 �� Scene��� Vector3 �ɕϊ�
            Vector2 coord2D = coordProp.vector2Value;
            Vector3 worldPos = new Vector3(coord2D.x, coord2D.y, 0f);

            // �n���h���̌����ڂ�T�C�Y
            const float handleSize = 0.2f;

            // �n���h����`�恕����
            EditorGUI.BeginChangeCheck();
            Vector3 newWorldPos = Handles.FreeMoveHandle(
                worldPos,
                handleSize,
                Vector3.zero,
                Handles.SphereHandleCap
            );
            // ���x�����o���Ƃ�
            string label = posEnumProp.enumDisplayNames[posEnumProp.enumValueIndex];
            Handles.Label(worldPos + Vector3.up * 0.3f, label);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(positioning, "Move PositionEntry Handle");
                // �X�V�������W�� Vector2 �ɖ߂��ăv���p�e�B�ɃZ�b�g
                coordProp.vector2Value = new Vector2(newWorldPos.x, newWorldPos.y);
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif
