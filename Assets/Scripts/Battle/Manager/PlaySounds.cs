using UnityEngine;

namespace RPG_003.Battle
{
    public class PlaySounds : MonoBehaviour
    {
        public enum PlaySound
        {
            BGM,
            Win,
            Lose,
        }
        [SerializeField] private AudioClip _inBattle;
        [SerializeField] private AudioClip _win;
        [SerializeField] private AudioClip _lose;
        AudioSource _playSound;
        public void Play(PlaySound sound)
        {
            switch (sound)
            {
                case PlaySound.BGM: _playSound.clip = _inBattle; _playSound.loop = true; break;
                case PlaySound.Win: _playSound.clip = _win; _playSound.loop = false; break;
                case PlaySound.Lose: _playSound.clip = _lose; _playSound.loop = false; break;
            }
            if (_playSound.clip != null)
                _playSound.Play();
        }

        private void Awake()
        {
            _playSound = GetComponent<AudioSource>();
        }
    }
}