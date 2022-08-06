using System.Collections.Generic;
using UnityEngine;
namespace Base.SoundManagement {
    [CreateAssetMenu(fileName = "New SFX", menuName = "Sound System/Music")]
    public class SoundCollection : SoundCollectionBase {

        public override void Initialize(in Transform MainSoundPlayer) {
            _soundDictionary = new Dictionary<string, Sound>();
            this.MainSoundPlayer = MainSoundPlayer;
            foreach (var sound in Collection) {
                if (!sound.Source) {
                    sound.Source = MainSoundPlayer.gameObject.AddComponent<AudioSource>();
                }
                sound.Source.playOnAwake = false;
                sound.Source.loop = false;
                sound.Source.clip = sound.SoundClip;
                sound.Source.volume = sound.Volume;
                sound.Source.pitch = sound.Pitch;


                _soundDictionary.Add(sound.SoundName, sound);
            }
        }

        public Sound Play(string soundName) {
            if (!_soundDictionary.ContainsKey(soundName)) {
                Debug.LogError("Sound with name " + soundName + " does not exist in this collection");
                return null;
            }
            var sound = _soundDictionary[soundName];
            sound.PlaySound();
            return sound;
        }
        public Sound Get(string soundName) {
            if (!_soundDictionary.ContainsKey(soundName)) {
                Debug.LogError("Sound with name " + soundName + " does not exist in this collection");
                return null;
            }
            return _soundDictionary[soundName];
        }
    }
}