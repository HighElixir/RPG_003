using RPG_003.Battle;
using Cysharp.Threading.Tasks;
using RPG_003.DataManagements.Datas;

namespace RPG_003.StatesEffect
{
    public abstract class StatesEffectBase : IStatesEffect
    {
        protected bool _shouldRemove = false;
        protected EffectData _effectData;
        public virtual bool ShouldRemove => _shouldRemove;
        public EffectData EffectData => _effectData;
        public virtual void OnAdd(Unit parent)
        {
            // Default implementation does nothing
        }
        public virtual void OnRemove(Unit parent)
        {
            // Default implementation does nothing
        }
        public virtual async UniTask Update(Unit parent)
        {
            // Default implementation does nothing
            await UniTask.Yield();
        }
        public void SetData(EffectData effectData) 
        {
            _effectData = effectData;
        }
        public abstract UniTask Create(string data);
    }

    public abstract class TimeBaseEffect : StatesEffectBase
    {
        // CSV
        // _defaultDuration;その他
        public int _defaultDuration = 0;
        protected int _remainingTime = 0;
        public override async UniTask Update(Unit parent)
        {
            _remainingTime--;
            if (_remainingTime <= 0)
            {
                _shouldRemove = true;
            }
            await base.Update(parent);
        }
    }
}