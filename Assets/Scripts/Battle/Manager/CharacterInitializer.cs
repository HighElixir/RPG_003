using RPG_003.Battle.Behaviour;
using RPG_003.Status;

namespace RPG_003.Battle
{
    /// <summary>
    /// キャラクターをシーンに生成して初期化する処理をまとめたクラス
    /// </summary>
    public static class CharacterInitializer
    {
        public static void InitCharacter(this BattleManager manage, CharacterObject c, CharacterData data, ICharacterBehaviour behaviour)
        {
            c.gameObject.name = data.Name;
            var statusMgr = new StatusManager(c, data);
            c.Initialize(data, statusMgr, behaviour, manage);
        }

        public static void InitPlayer(this BattleManager manage, Player c, PlayerData playerData, CharacterData characterData, ICharacterBehaviour behaviour)
        {
            c.SetPlayerData(playerData);
            InitCharacter(manage, c, characterData, behaviour);
        }
    }
}
