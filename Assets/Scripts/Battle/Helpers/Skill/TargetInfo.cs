using RPG_003.Battle.Characters;
using System.Collections.Generic;

namespace RPG_003.Battle
{
    public class TargetInfo
    {
        public ICharacter MainTarget { get; private set; } // メインターゲット
        public List<ICharacter> AdditionalTargets { get; private set; } // 追加ターゲット

        public void SetMainTarget(ICharacter target)
        {
            MainTarget = target;
        }
        public void AddAdditionalTarget(ICharacter target)
        {
            if (AdditionalTargets == null)
            {
                AdditionalTargets = new List<ICharacter>();
            }
            AdditionalTargets.Add(target);
        }

        public void AddAdditionalTargets(List<ICharacter> targets)
        {
            if (AdditionalTargets == null)
            {
                AdditionalTargets = new List<ICharacter>();
            }
            AdditionalTargets.AddRange(targets);
        }

        public List<ICharacter> CloneAndConvert()
        {
            var reslut = new List<ICharacter>();
            reslut.Add(MainTarget);
            reslut.AddRange(AdditionalTargets);
            return reslut;
        }
        public TargetInfo(ICharacter mainTarget, List<ICharacter> additionalTargets = null)
        {
            MainTarget = mainTarget;
            AdditionalTargets = additionalTargets ?? new List<ICharacter>();
        }
    }
}