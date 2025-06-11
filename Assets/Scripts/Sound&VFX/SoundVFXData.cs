using UnityEngine;

namespace RPG_003.Effect
{
    [CreateAssetMenu(fileName = "SoundVFXData", menuName = "RPG_003/UI/SoundAndVFX", order = 1)]
    public class SoundVFXData : ScriptableObject
    {
        [SerializeField] private AudioClip _audioClip;
        [SerializeField] private ParticleSystem _VFX;

        public AudioClip AudioClip => _audioClip;
        public ParticleSystem VFX => _VFX;
    }
}