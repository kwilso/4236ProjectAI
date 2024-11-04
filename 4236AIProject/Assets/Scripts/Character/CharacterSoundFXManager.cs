using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KW {
    public class CharacterSoundFXManager : MonoBehaviour
    {
        private AudioSource audioSource;

        protected virtual void Awake() {
            audioSource = GetComponent<AudioSource>();
        }

        public void PlayRollSFX() {
            audioSource.PlayOneShot(WorldSoundFXManager.instance.rollSFX);
        }
    }
}
