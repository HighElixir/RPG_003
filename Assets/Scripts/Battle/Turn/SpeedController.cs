using RPG_001.Battle.Characters;
using UnityEngine;

namespace RPG_001.Battle
{
    // StatusManager����ɏ�����
    public class SpeedController
    {
        private StatusAmount _speedAmount;
        private int _curretAmount = 0;

        public bool IsReady => _curretAmount <= 0;
        public int CurrentAmount => _curretAmount;
        public int Max
        {
            get
            {
                // (500 - x) / 400 ���܂��v�Z
                float t = (500.0f - _speedAmount.ChangedMax) / 400.0f;
                // �x�L�悵�ăX�P�[����������
                return (int)(10.0f + 140.0f * Mathf.Pow(t, 2.3f));
            }
        }

        public void Process(int amount)
        {
            _curretAmount = Mathf.Max(0, _curretAmount - amount);
        }
        
        public void Initialize(StatusAmount speedAmount)
        {
            _speedAmount = speedAmount;
            _curretAmount = 0;
        }
    }
}