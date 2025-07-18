﻿// Assets/Editor/CountableSwitchEditor.cs
using UnityEngine;
using UnityEditor;
using HighElixir.UI;

namespace HighElixir.UI.Editor
{
    [CustomEditor(typeof(CountableSwitch))]
    public class CountableSwitchEditor : UnityEditor.Editor
    {
        // SerializedProperty のキャッシュ
        SerializedProperty _minusProp;
        SerializedProperty _plusProp;
        SerializedProperty _textProp;

        SerializedProperty _soundOptionProp;
        SerializedProperty _clickSoundProp;
        SerializedProperty _disallowedSoundProp;
        SerializedProperty _minusSoundProp;
        SerializedProperty _plusSoundProp;

        SerializedProperty _defaultAmountProp;
        SerializedProperty _oneClickChangeProp;
        SerializedProperty _minProp;
        SerializedProperty _maxProp;

        SerializedProperty _onChangedProp;

        void OnEnable()
        {
            var so = serializedObject;
            _minusProp = so.FindProperty("_minus");
            _plusProp = so.FindProperty("_plus");
            _textProp = so.FindProperty("_text");

            _soundOptionProp = so.FindProperty("_soundOption");
            _clickSoundProp = so.FindProperty("_clickSound");
            _disallowedSoundProp = so.FindProperty("_disallowedSound");
            _minusSoundProp = so.FindProperty("_minusSound");
            _plusSoundProp = so.FindProperty("_plusSound");

            _defaultAmountProp = so.FindProperty("_defaultAmount");
            _oneClickChangeProp = so.FindProperty("_oneClickChange");
            _minProp = so.FindProperty("min");
            _maxProp = so.FindProperty("max");

            _onChangedProp = so.FindProperty("_onChanged");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.LabelField("References", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_minusProp);
            EditorGUILayout.PropertyField(_plusProp);
            EditorGUILayout.PropertyField(_textProp);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Audio Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_soundOptionProp);

            // 失敗時クリック音は常に表示
            EditorGUILayout.PropertyField(_disallowedSoundProp, new GUIContent("Disallowed Sound"));

            // soundOption に応じて表示を切り替え
            var mode = (CountableSwitch.ClickSoundOp)_soundOptionProp.enumValueIndex;
            if (mode == CountableSwitch.ClickSoundOp.One)
            {
                EditorGUILayout.PropertyField(_clickSoundProp, new GUIContent("Click Sound"));
            }
            else // Two
            {
                EditorGUILayout.PropertyField(_minusSoundProp, new GUIContent("Minus Sound"));
                EditorGUILayout.PropertyField(_plusSoundProp, new GUIContent("Plus  Sound"));
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Data", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_defaultAmountProp, new GUIContent("Default Amount"));
            EditorGUILayout.PropertyField(_oneClickChangeProp, new GUIContent("One Click Change"));
            EditorGUILayout.PropertyField(_minProp, new GUIContent("Min Value"));
            EditorGUILayout.PropertyField(_maxProp, new GUIContent("Max Value"));

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(_onChangedProp);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
