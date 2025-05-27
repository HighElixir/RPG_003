using RPG_001.Battle.Characters;
using UnityEngine;

public class StatusDebugViewer : MonoBehaviour
{
    [SerializeField] private CharacterBase _target;

    private ICharacter targetCharacter;

    private void OnValidate()
    {
        // �C���X�y�N�^�[�ō����ւ����Ƃ��ɍăL���X�g
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
        GUILayout.Label("�X�e�[�^�X�\���f�o�b�O");

        if (targetCharacter == null)
        {
            GUILayout.Label("�L�����N�^�[���w��I");
        }
        else if (GUILayout.Button("�X�e�[�^�X��\��"))
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
            Debug.LogWarning("StatusManager ��������Ȃ���I");
            return;
        }

        Debug.Log($"--- {targetCharacter.Data.Name} �̃X�e�[�^�X ---");
        foreach (var status in statusManager.GetStatusList())
        {
            var amount = statusManager.GetStatusAmount(status);
            Debug.Log($"{status} : {amount.currentAmount} / Max: {amount.ChangedMax} (Base: {amount.defaultAmount})");
        }
        Debug.Log($"------------------------------");
    }
}
