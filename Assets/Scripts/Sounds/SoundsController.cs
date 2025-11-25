using System.Collections.Generic;
using UnityEngine;

namespace Sounds
{
    public class SoundsController : MonoBehaviour
    {
        [SerializeField] 
        private List<AudioSource> _audioSources;

        public void TryPlaySound(AudioClip clip)
        {
            if (!SoundsStates.CanPlaySound)
            {
                return;
            }

            int index = _audioSources[0].isPlaying ? 1 : 0;

            _audioSources[index].clip = clip;
            _audioSources[index].Play();
        }
    }
}