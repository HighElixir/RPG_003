using Cysharp.Threading.Tasks;
using RPG_003.Character;
using RPG_003.Skills;
using RPG_003.DataManagements.Datas;
using System;
using UnityEngine;
using UnityEngine.UI;
namespace RPG_003.Tutor
{
    [Serializable]
    public class BattleEnter : ITutor
    {
        [SerializeField] private Toggle _skillMakerSwitch;
        [SerializeField] private Button _prefab;
        [SerializeField] private BasicData _fire;
        [SerializeField] private CharacterDataHolder _character;
        [SerializeField] private StatusData _status;

        public async UniTask Script()
        {
            _skillMakerSwitch.isOn = true;
            _skillMakerSwitch.enabled = false;
        }
    }
}