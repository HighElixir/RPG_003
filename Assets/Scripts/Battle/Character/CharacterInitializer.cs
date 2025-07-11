using RPG_003.Battle.Behaviour;
using RPG_003.Status;
using UnityEngine.XR;

namespace RPG_003.Battle
{
    /// <summary>
    /// キャラクターをシーンに生成して初期化する処理をまとめたクラス
    /// </summary>
    public static class CharacterInitializer
    {

        public static Unit InitPlayer(this Unit c, BattleManager manage, PlayerData playerData)
        {
            InitCharacter(c, manage, playerData.StatusData);
            c.SetSkills(Skill.CreateSkills(playerData.Skills).SetParent(c));
            c.SetBehaivior(new PlayerBehaviour().Initialize(c, manage));
            if (playerData.Icon != null)
                c.SetIcon(playerData.Icon);
            return c;
        }
        public static Unit InitEnemy(this Unit c, BattleManager manage, EnemyData enemy)
        {
            InitCharacter(c, manage, enemy.statusData);
            enemy.skill.Initialize();
            c.SetSkills(enemy.skill.Skills);
            c.SetBehaivior(new AIBehavior().SetSkill(enemy.skill).Initialize(c, manage));
            if (enemy.icon != null)
                c.SetIcon(enemy.icon);
            return c;
        }
        private static void InitCharacter(Unit c, BattleManager manage, StatusData data)
        {
            c.gameObject.name = data.Name;
            c.Initialize(manage).SetStatus(data);
        }
    }
}
