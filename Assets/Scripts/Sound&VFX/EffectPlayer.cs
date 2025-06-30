using System.Collections;
using UnityEngine;

namespace RPG_003.Effect
{
    [RequireComponent(typeof(AudioSource))]
    public class EffectPlayer : MonoBehaviour
    {
        [SerializeField] private AudioSource _AudioSource;

        public IEnumerator Play(SoundVFXData vFXData, Vector2 position)
        {

            if (vFXData.AudioClip != null)
            {
                var c = vFXData.AudioClip;
                _AudioSource.clip = c;
                _AudioSource.Play();
            }
            if (vFXData.VFX != null)
            {
                var v = Instantiate(vFXData.VFX, position, Quaternion.identity);
                yield return new WaitWhile(() => v != null && v.IsAlive());
                if (v != null)
                    Destroy(v.gameObject);
            }
            yield break;
        }

        private void Awake()
        {
            _AudioSource = GetComponent<AudioSource>();
        }
    }
}