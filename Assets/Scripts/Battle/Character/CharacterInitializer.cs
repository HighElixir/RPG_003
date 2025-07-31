using RPG_003.Battle.Behaviour;
using RPG_003.DataManagements.Datas;

namespace RPG_003.Battle
{
    /// <summary>
    /// キャラクターをシーンに生成して初期化する処理をまとめたクラス
    /// </summary>
    public static class CharacterInitializer
    {

        public static Unit InitPlayer(this Unit c, BattleManager manage, PlayerData playerData)
        {
            InitCharacter(c, manage, playerData.statusData);
            c.SetSkills(Skill.CreateSkills(playerData.skillDataInBattles).SetParent(c));
            var b = new PlayerBehaviour().Initialize(manage);
            c.SetBehaivior(b);
            c.OnDeath += b.OnDeath;

            if (playerData.icon != null)
                c.SetIcon(playerData.icon);
            return c;
        }
        public static Unit InitEnemy(this Unit c, BattleManager manage, EnemyData enemy)
        {
            InitCharacter(c, manage, enemy.statusData);
            enemy.Behaviour.Initialize();
            c.SetSkills(enemy.Behaviour.Skills);
            var b = new AIBehavior().SetBehaviour(enemy.Behaviour).Initialize(manage);
            c.SetBehaivior(b);
            c.OnDeath += b.OnDeath;
            if (enemy.icon != null)
            {
                c.SetIcon(enemy.icon);
            }
            return c;
        }
        private static void InitCharacter(Unit c, BattleManager manage, StatusData data)
        {
            c.gameObject.name = data.Name;
            c.Initialize(manage).SetStatus(data);
        }
    }
}
