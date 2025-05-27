using TMPro;
using UnityEngine;

namespace Minigames.UI
{
    public interface IPopTextData
    {
        public TMP_FontAsset FontAsset { get; }

        /// <summary>
        /// �͈͊O��idx�̏ꍇ�̓f�t�H���g�l��Ԃ�.
        /// </summary>
        public (string text, Color color) GetInfo(int id);
    }
}