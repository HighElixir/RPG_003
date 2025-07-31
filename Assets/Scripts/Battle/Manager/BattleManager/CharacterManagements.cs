using Cysharp.Threading.Tasks;
using RPG_003.DataManagements.Enum;
using System.Collections.Generic;
using UnityEngine;

namespace RPG_003.Battle
{
    public partial class BattleManager
    {
        // 
        public async UniTask<List<Unit>> SummonPlayers(List<PlayerData> players)
        {
            var res = new List<Unit>();
            int count = Mathf.Min(players.Count, 4);
            var units = await InstantiateAsync(_unitPrefab, count, Vector3.zero, Quaternion.identity).ToUniTask();
            for (int i = 0; i < count; i++)
            {
                units[i].InitPlayer(this, players[i]);
                res.Add(units[i]);
            }
            return res;
        }
        public async UniTask<List<Unit>> SummonEnemies(List<EnemyData> enemies)
        {
            var result = new List<Unit>();
            int count = Mathf.Min(enemies.Count, 4);

            var units = await InstantiateAsync(_unitPrefab, count, Vector3.zero, Quaternion.identity).ToUniTask();

            for (int i = 0; i < count; i++)
            {
                var enemyData = enemies[i];

                // 空き位置を取得
                if (!_posManager.TryGetUsablePosition(out var characterPosition, Faction.Enemy))
                {
                    Debug.LogError($"モンスター「{enemyData.enemyName}」のスポーンに失敗！");
                    continue;
                }

                var unit = units[i];
                unit.InitEnemy(this, enemyData);

                GraphicalManager.instance.BattleLog.Add(
                    $"<color=purple>{enemyData.enemyName}</color>が現れた！",
                    BattleLog.IconType.Normal
                );

                RegisterCharacter(characterPosition, unit);
                result.Add(unit);
            }

            return result;
        }

        // デフォルトの場合、空いてる場所にスポーンさせる
        public void SummonEnemy(EnemyData enemyData, CharacterPosition characterPosition = CharacterPosition.None)
        {
            if (characterPosition == CharacterPosition.None && !_posManager.TryGetUsablePosition(out characterPosition, Faction.Enemy))
            {
                Debug.LogError("モンスターのスポーンに失敗！");
                return;
            }
            var c = Instantiate(_unitPrefab, Vector3.zero, Quaternion.identity);
            c.InitEnemy(this, enemyData);

            GraphicalManager.instance.BattleLog.Add($"<color=purple>{enemyData.enemyName}</color>が現れた！", BattleLog.IconType.Normal);
            RegisterCharacter(characterPosition, c);
        }
        // 各派生クラスにキャラクターを登録する
        public void RegisterCharacter(CharacterPosition position, Unit character)
        {
            character.transform.SetParent(_charactersContainer);
            _characterTransformHelper.SetPosition(character, position);
            _posManager.RegisterCharacter(position, character);
            GraphicalManager.instance.TakeName(character);
            Debug.Log($"Starting battle with player: {character.Data.Name} at position: {position}");
        }

        // キャラをゲームから削除
        public void RemoveCharacter(Unit character)
        {
            // PositionManager で位置削除
            _posManager.RemoveCharacter(character);
            _turnManager.RemoveCharacter(character);
            OnCharacterRemoved?.Invoke(character);
            CheckBattleEnd();
            Destroy(character.gameObject);
        }
        public bool TryGetUsablePosition(out CharacterPosition position, Faction faction = Faction.All)
        {
            return _posManager.TryGetUsablePosition(out position, faction);
        }

        public List<Unit> GetCharacters()
        {
            return _posManager.GetCharacters();
        }

        public IReadOnlyDictionary<CharacterPosition, Unit> GetCharacterMap()
        {
            return _posManager.GetCharacterMap();
        }
    }
}