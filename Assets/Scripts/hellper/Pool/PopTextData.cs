using TMPro;
using UnityEngine;

namespace Minigames.UI
{
    public abstract class PopTextData : ScriptableObject, IPopTextData
    {
        public virtual TMP_FontAsset FontAsset { get; }

        /// <summary>
        /// �͈͊O��idx�̏ꍇ�̓f�t�H���g�l��Ԃ�.
        /// </summary>
        public abstract (string text, Color color) GetInfo(int id);
    }
}