using RPG_003.Status;
using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace RPG_003.Battle
{
#if UNITY_EDITOR
    public class StatusDebugViewer : SerializedMonoBehaviour
    {
        [SerializeField] private Unit _target;
        private bool _isTargetSet = false;
        private bool _showStatus = false;

        [SerializeField, ShowIf("_showStatus")]
        private Dictionary<string, float> _statusValues = new();

        [ShowIf("_isTargetSet")]
        [Button("Toggle Status")]
        private void ShowStatus()
        {
            // トグル機能: 表示状態を切り替え
            if (_showStatus)
            {
                _statusValues.Clear();  // 前回の値をクリア
                _showStatus = false;
                GUIHelper.RequestRepaint();  // インスペクタを更新
                return;
            }

            if (_isTargetSet)
            {
                _statusValues.Clear();
                foreach (var status in _target.StatusManager.GetStatusList())
                {
                    var amount = _target.StatusManager.GetStatusAmount(status);
                    if (status.Equals(StatusAttribute.HP) || status.Equals(StatusAttribute.MP))
                    {
                        _statusValues["Max" + status] = amount.ChangedMax;
                        _statusValues[status.ToString()] = amount.currentAmount;
                    }
                    else
                    {
                        _statusValues[status.ToString()] = amount.ChangedMax;
                    }
                }
                _showStatus = true;
                GUIHelper.RequestRepaint();
            }
        }
        [ShowIf("_showStatus")]
        [Button("Set Status")]
        private void Set()
        {
            var sM = _target.StatusManager;
            foreach (var kvp in _statusValues)
            {
                bool flag = false;
                string key = kvp.Key;
                if (kvp.Key.Contains("Max"))
                {
                    key = key.Trim("Max".ToCharArray());
                    flag = true;
                }
                if (sM.TryGetStatus((StatusAttribute)Enum.Parse(typeof(StatusAttribute), key), out var s))
                {
                    s.Debug(kvp.Value, flag);
                }
            }
        }
        private void OnValidate()
        {
            _isTargetSet = _target != null;
        }
    }
#endif
}