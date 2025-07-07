using Cysharp.Threading.Tasks;
using UnityEngine;

namespace RPG_003.Effect
{
    [RequireComponent(typeof(AudioSource))]
    public class EffectPlayer : MonoBehaviour
    {
        [SerializeField] private AudioSource _AudioSource;

        public async UniTask Play(SoundVFXData vFXData, Vector2 position)
        {
            if (vFXData.AudioClip != null)
            {
                var c = vFXData.AudioClip;
                _AudioSource.clip = c;
                _AudioSource.Play();
            }
            if (vFXData.VFX != null)
            {
                ParticleSystem v = Instantiate(vFXData.VFX, position, Quaternion.identity);
                await UniTask.WaitWhile(() => v != null && v.IsAlive());
                if (v != null)
                    Destroy(v.gameObject);
            }
        }

        private void Awake()
        {
            _AudioSource = GetComponent<AudioSource>();
        }
    }
}