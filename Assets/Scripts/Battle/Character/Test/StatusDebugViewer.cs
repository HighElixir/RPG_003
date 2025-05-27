using RPG_001.Battle.Characters;
using UnityEngine;

public class StatusDebugViewer : MonoBehaviour
{
    [SerializeField] private CharacterBase _target;

    private ICharacter targetCharacter;

    private void OnValidate()
    {
        // インスペクターで差し替えたときに再キャスト
        if (_target is ICharacter character)
        {
            targetCharacter = character;
        }
        else
        {
            targetCharacter = null;
        }
    }

    private void OnGUI()
    {
        GUILayout.BeginVertical("box");
        GUILayout.Label("ステータス表示デバッグ");

        if (targetCharacter == null)
        {
            GUILayout.Label("キャラクター未指定！");
        }
        else if (GUILayout.Button("ステータスを表示"))
        {
            ShowCharacterStatus();
        }

        GUILayout.EndVertical();
    }

    private void ShowCharacterStatus()
    {
        var statusManager = targetCharacter.StatusManager;
        if (statusManager == null)
        {
            Debug.LogWarning("StatusManager が見つからないよ！");
            return;
        }

        Debug.Log($"--- {targetCharacter.Data.Name} のステータス ---");
        foreach (var status in statusManager.GetStatusList())
        {
            var amount = statusManager.GetStatusAmount(status);
            Debug.Log($"{status} : {amount.currentAmount} / Max: {amount.ChangedMax} (Base: {amount.defaultAmount})");
        }
        Debug.Log($"------------------------------");
    }
}
