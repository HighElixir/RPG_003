using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using RPG_003.Helper;
using RPG_003.StatesEffect;
namespace RPG_003.Battle
{
    public class StatesEffectController
    {
        public readonly Unit parent;

        private List<IStatesEffect> _effects = new List<IStatesEffect>();
        public StatesEffectController(Unit parent)
        {
            this.parent = parent;
        }

        public async UniTask Update()
        {
            foreach (var effect in _effects)
            {
                await effect.Update(parent);
            }
            _effects.RemoveAll(effect =>
            {
                effect.OnRemove(parent);
                return effect.ShouldRemove;
            });
        }

        public void AddEffect(IStatesEffect effect)
        {
            if (effect == null) return;
            if (effect is IStackable stackable)
            {
                // スタック可能な効果の場合、同じIDの効果が存在するか確認する。
                var existing = _effects.FirstOrDefault(e => e.Id == effect.Id);
                if (existing != null && existing is IStackable existingStackable)
                {
                    existingStackable.AddStack(stackable);
                    return;
                }
            }
            
            _effects.Add(effect);
            effect.OnAdd(parent);
        }
        public void RemoveEffect(IStatesEffect effect)
        {
            if (effect == null) return;
            if (_effects.Remove(effect))
            {
                effect.OnRemove(parent);
            }
        }
    }
}