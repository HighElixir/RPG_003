using RPG_003.Battle.Behaviour;
using RPG_003.Battle.Characters;
using RPG_003.Battle.Characters.Player;

namespace RPG_003.Battle
{
    /// <summary>
    /// キャラクターをシーンに生成して初期化する処理をまとめたクラス
    /// </summary>
    public class CharacterInitializer
    {
        private BattleManager _battleManager;

        public CharacterInitializer(BattleManager battleManager)
        {
            _battleManager = battleManager;
        }

        public void InitCharacter(CharacterBase c, CharacterData data, ICharacterBehaviour behaviour)
        {
            c.gameObject.name = data.Name;
            var statusMgr = new StatusManager(c, data);
            c.Initialize(data, statusMgr, behaviour, _battleManager);
        }

        public void InitPlayer(Player c, PlayerData playerData, CharacterData characterData, ICharacterBehaviour behaviour)
        {
            c.SetPlayerData(playerData);
            InitCharacter(c, characterData, behaviour);
        }
    }
}
