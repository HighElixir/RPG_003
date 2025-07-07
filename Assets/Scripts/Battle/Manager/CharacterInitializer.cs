using RPG_003.Battle.Behaviour;
using RPG_003.Status;

namespace RPG_003.Battle
{
    /// <summary>
    /// キャラクターをシーンに生成して初期化する処理をまとめたクラス
    /// </summary>
    public static class CharacterInitializer
    {
        public static void InitCharacter(this Unit c, BattleManager manage, StatusData data)
        {
            c.gameObject.name = data.Name;
            c.Initialize(manage).SetStatus(data);
        }

        public static Unit InitPlayer(this Unit c, BattleManager manage, PlayerData playerData)
        {
            c.InitCharacter(manage, playerData.StatusData);
            c.SetSkills(Skill.CreateSkills(playerData.Skills).SetParent(c));
            if (playerData.Icon != null)
                c.SetIcon(playerData.Icon);
            return c;
        }
        public static Unit InitEnemy(this Unit c, BattleManager manage, EnemyData enemy)
        {
            c.InitCharacter(manage, enemy.statusData);
            c.SetSkills(enemy.enemyBehaviorData.Skills);
            c.SetBehaivior(enemy.enemyBehaviorData.GetCharacterBehaviour());
            if (enemy.icon != null)
                c.SetIcon(enemy.icon);
            return c;
        }
    }
}
