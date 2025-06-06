using RPG_003.Battle.Characters;
using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;
using System.Collections.Generic;
using UnityEngine;

public class StatusDebugViewer : SerializedMonoBehaviour
{
    [SerializeField] private CharacterBase _target;
    private bool _isTargetSet = false;
    private bool _showStatus = false;

    [SerializeField, ShowIf("_showStatus")]
    private Dictionary<string, float> _statusValues = new Dictionary<string, float>();

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

    private void OnValidate()
    {
        _isTargetSet = _target != null;
    }
}
