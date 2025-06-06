using UnityEngine;
using RPG_003.Battle.Characters;
using RPG_003.Battle.Behaviour;

namespace RPG_003.Battle
{
    /// <summary>
    /// キャラクターをシーンに生成して初期化する処理をまとめたクラス
    /// </summary>
    public class CharacterInitializer
    {
        private Camera _camera;
        private IntervalIndicator _intervalIndicatorPrefab;
        private PositionManager _positionManager;
        private Positioning _positioning;
        private Transform _canvas;
        private Transform _charactersContainer;
        private IBattleManager _battleManager;

        public CharacterInitializer(
            Camera camera,
            IntervalIndicator intervalIndicatorPrefab,
            PositionManager positionManager,
            Positioning positioning,
            Transform canvas,
            Transform charactersContainer,
            IBattleManager battleManager)
        {
            _camera = camera;
            _intervalIndicatorPrefab = intervalIndicatorPrefab;
            _positionManager = positionManager;
            _positioning = positioning;
            _canvas = canvas;
            _charactersContainer = charactersContainer;
            _battleManager = battleManager;
        }

        /// <summary>
        /// c: 生成済み／Instantiate された CharacterBase（もしくは Player）
        /// data: 設定する CharacterData
        /// pos: BattleManager から渡される指定ポジション
        /// behaviour: ICharacterBehaviour の実装
        /// statusMgr: BattleManager が渡す StatusManager（もしくは IStatusManager 実装）
        /// </summary>
        public void InitCharacter(
            CharacterBase c,
            IStatusManager statusMgr,
            CharacterData data,
            CharacterPosition pos,
            ICharacterBehaviour behaviour)
        {
            // 元の InitCharacter とほぼ同じ中身をコピー。ただし
            // - 名前の付与
            // - ディクショナリへの登録は BattleManager でやるから、返り値でポジションを渡すか、
            //   ここで直接参照しても OK（参照渡し）。
            c.gameObject.name = data.Name;
            c.Position = pos;
            statusMgr.Initialize(c, data);

            // インターバルインジケータを生成して Canvas に配置
            var worldPos = _positioning.GetPosition(pos);

            var screenPoint = RectTransformUtility.WorldToScreenPoint(_camera, worldPos);

            var spd = Object.Instantiate(
                _intervalIndicatorPrefab,
                screenPoint,
                Quaternion.identity,
                _canvas);

            c.Initialize(data, statusMgr, behaviour, _battleManager, _intervalIndicatorPrefab);
            // ← IBattleManager は後で BattleManager がセットし直すから、ここでは null でも大丈夫。ただし
            //   あとで上書きしないと動かないので注意してね。

            // Container に追加
            c.transform.position = worldPos;
            c.transform.SetParent(_charactersContainer);
        }
    }
}
