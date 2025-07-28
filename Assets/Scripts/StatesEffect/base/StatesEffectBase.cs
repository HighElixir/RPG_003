using UnityEngine;
using RPG_003.Battle;
using Cysharp.Threading.Tasks;

namespace RPG_003.StatesEffect
{
    public abstract class StatesEffectBase : IStatesEffect
    {
        protected bool _shouldRemove = false;
        [SerializeField] private int _id = 0;
        [SerializeField] private string _name = string.Empty;
        [SerializeField] private string _description = string.Empty;
        [SerializeField] private Sprite _icon = null;
        [SerializeField] private bool _isPositive = true; // Default to positive effect
        public virtual bool ShouldRemove => _shouldRemove;
        public int Id => _id;
        public string Name => _name;
        public string Description => _description;
        public virtual string OnAddedMessage => $"{Name}が@Targetに付与されました。";
        public Sprite Icon => _icon;
        public bool IsPositive => _isPositive;
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

        // Chain
        #region
        public T SetID<T>(int id) where T : StatesEffectBase
        {
            _id = id;
            return this as T;
        }
        public T SetName<T>(string name) where T : StatesEffectBase
        {
            _name = name;
            return this as T;
        }
        public T SetDescription<T>(string description) where T : StatesEffectBase
        {
            _description = description;
            return this as T;
        }
        public T SetIcon<T>(Sprite icon) where T : StatesEffectBase
        {
            _icon = icon;
            return this as T;
        }
        #endregion
    }

    public abstract class TimeBaseEffect : StatesEffectBase
    {
        [SerializeField] protected int _defaultDuration = 0;
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

        // Chains
        public TimeBaseEffect SetDuration(int duration)
        {
            _defaultDuration = duration;
            return this;
        }
    }
}