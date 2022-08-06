using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Base;
using DG.Tweening;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;

public abstract class SoundCollectionBase : ScriptableObject {

    public string CollectionName;
    public Sound[] Collection;

    protected Dictionary<string, Sound> _soundDictionary;

    protected Transform MainSoundPlayer;

    public abstract void Initialize(in Transform MainSoundPlayer);

    #if UNITY_EDITOR

    [Button("Auto Setup")]
    private void CreateEnums() {
        var names = new List<string>();
        for (var i = 0; i < Collection.Length; i++) {
            if (Collection[i].PullName) {
                Collection[i].SoundName = Collection[i].SoundClip.name;
            }

            names.Add(Collection[i].SoundName);
            if (names.Count <= 0) Debug.LogWarning("No Sounds Found, Please Add Sounds");
            else continue;
        }
        B_EnumCreator.CreateEnum($"{CollectionName}", names.ToArray());
    }

    #endif

    #region Save

    [SerializeField]
    private ScriptableObjectSaveInfo SaveInfo;

    [Button]
    void Save() {
        SaveInfo.ModifyInfo(this, "Saves/Sounds", $"{CollectionName}_Collection");
        SaveInfo.SaveScriptableObject();
    }

    [Button]
    void load() {
        SaveInfo.LoadScriptableObject();
    }

    #endregion

}

[Serializable]
public class Sound {

    public Action OnSoundEnded;
    public string SoundName;
    public AudioClip SoundClip;
    [HideInInspector]
    public AudioSource Source;
    [Range(0f, 1f)]
    public float Volume;
    [Range(0.1f, 3f)]
    public float Pitch;

    public bool PullName;

    Coroutine _playCoroutine;

    public bool isPlaying { get; private set; }
    public bool isLooping { get; private set; }
    public bool isLocked { get; private set; }
    public bool isStopGiven { get; private set; }

    public Sound(AudioClip clip, float volume, float pitch) {
        SoundClip = clip;
        Volume = volume;
        Pitch = pitch;
    }

    public Sound PlaySound() {
        if (isLocked) return this;
        CancelOrder();
        _playCoroutine = PlaySoundRoutine().RunCoroutine();
        return this;
    }

    public Sound StopSound() {
        if (isLocked) return this;
        isStopGiven = true;
        return this;
    }

    public Sound StopSoundImmediately() {
        if (isLocked) return this;
        CancelOrder();
        Source.Stop();
        Source.time = 0;
        return this;
    }

    public Sound UnlockSound() {
        if (isLocked) {
            isLocked = false;
            return this;
        }
        return this;
    }

    public Sound LockSound() {
        if (!isLocked) {
            isLocked = true;
            return this;
        }
        return this;
    }

    public Sound SetDelay(float delay) {
        if (isLocked) return this;
        CancelOrder();
        _playCoroutine = PlaySoundRoutine(delay).RunCoroutine();
        return this;
    }

    public Sound SetLoop(int loop) {
        if (isLocked) return this;
        CancelOrder();
        _playCoroutine = PlaySoundRoutine(loop).RunCoroutine();
        return this;
    }

    public Sound SetLoop(int loop, float delayBetweenLoops) {
        if (isLocked) return this;
        CancelOrder();
        _playCoroutine = PlaySoundRoutine(loop, delayBetweenLoops).RunCoroutine();
        return this;
    }

    public Sound SetLoop(int loop, float delayBetweenLoops, float initialDelay) {
        if (isLocked) return this;
        CancelOrder();
        _playCoroutine = PlaySoundRoutine(loop, delayBetweenLoops, initialDelay).RunCoroutine();
        return this;
    }

    public Sound SetSoundSource(AudioSource source) {
        Source = source;
        return this;
    }

    public Sound SetSoundSource(GameObject obj) {
        if (obj.GetComponent<AudioSource>() == null) {
            obj.AddComponent<AudioSource>();
            Source = obj.GetComponent<AudioSource>();
            Source.clip = SoundClip;
            Source.volume = Volume;
            Source.pitch = Pitch;
            Source.loop = false;
            Source.playOnAwake = false;
        }
        else {
            Source = obj.GetComponent<AudioSource>();
            Source.clip = SoundClip;
            Source.volume = Volume;
            Source.pitch = Pitch;
            Source.loop = false;
            Source.playOnAwake = false;
        }
        return this;
    }

    public Sound SetSoundSource(Transform obj) {
        if (obj.GetComponent<AudioSource>() == null) {
            obj.gameObject.AddComponent<AudioSource>();
            Source = obj.GetComponent<AudioSource>();
        }
        else {
            Source = obj.GetComponent<AudioSource>();
        }
        return this;
    }

    public Sound SetVolume(float volume) {
        Volume = volume;
        this.Source.volume = volume;
        return this;
    }

    public Sound SetPitch(float pitch) {
        Pitch = pitch;
        this.Source.pitch = pitch;
        return this;
    }

    IEnumerator PlaySoundRoutine() {
        Source.Play();
        isPlaying = true;
        yield return new WaitForSeconds(SoundClip.length);
        isPlaying = false;
        OnSoundEnded?.Invoke();
        _playCoroutine = null;
    }

    IEnumerator PlaySoundRoutine(float delay) {
        isPlaying = true;
        yield return new WaitForSeconds(delay);
        Source.Play();
        yield return new WaitForSeconds(SoundClip.length);
        isPlaying = false;
        OnSoundEnded?.Invoke();
        _playCoroutine = null;
    }

    IEnumerator PlaySoundRoutine(int Loop, [CanBeNull] float delayBetween = 0f, [CanBeNull] float delay = 0f) {
        yield return new WaitForSeconds(delay);
        if (Loop <= 0) {
            yield return InfiniteLoop(delayBetween);
        }
        for (int i = 0; i < Loop; i++) {
            Source.Play();
            yield return new WaitForSeconds(SoundClip.length + delayBetween);
            if (isStopGiven) {
                isStopGiven = false;
                StopSoundImmediately();
                break;
            }
        }
        isPlaying = false;
        isLooping = false;
        OnSoundEnded?.Invoke();
        _playCoroutine = null;
    }
    
    int _loop = 0;

    IEnumerator InfiniteLoop([CanBeNull] float delayBetween = 0f) {
        _loop = 0;
        while (true) {
            Source.Play();
            yield return new WaitForSeconds(Source.clip.length + delayBetween);
            yield return new WaitForSeconds(.01f);
            if (isStopGiven) {
                isStopGiven = false;
                StopSoundImmediately();
                break;
            }
        }
    }

    void CancelOrder() {
        _playCoroutine?.StopCoroutine();
        Source.Stop();
        isPlaying = false;
        isLooping = false;
        isLocked = false;
    }

}