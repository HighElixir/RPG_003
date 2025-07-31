using Cysharp.Threading.Tasks;
using RPG_003.Battle;
using RPG_003.DataManagements.Datas;
using UnityEngine;

namespace RPG_003.StatesEffect
{
    /// <summary>
    /// 自分自身を取り除く処理は避けること。
    /// </summary>
    public interface IStatesEffect
    {
        /// <summary>
        /// trueならば、Update()の後にRemoveEffect()が呼ばれる。
        /// </summary>
        bool ShouldRemove { get; }
        EffectData EffectData { get; }
        UniTask Update(Unit parent);
        void OnAdd(Unit parent);
        void OnRemove(Unit parent);

        UniTask Create(string data);
    }

    public interface IStackable : IStatesEffect
    {
        /// <summary>
        /// スタック数を取得する。
        /// </summary>
        int StackCount { get; }
        /// <summary>
        /// スタック数を追加する。
        /// </summary>
        void AddStack(IStackable other);
    }
}