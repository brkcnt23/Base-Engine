using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
namespace Base {
    public class B_PooledParticle : MonoBehaviour {

        public Action OnComplete;
        private ParticleSystem _particleSystem;
        private List<ParticleSystem> _particleSystems = new List<ParticleSystem>();


        private Coroutine _playEffectCoroutine;

        private int _loop;

        public bool _isPlaying { get; private set; }
        public bool _isLooping { get; private set; }
        public bool _isLocked { get; private set; }
        public bool _isStopGiven { get; private set; }

        public void OnFirstSpawn() {
            SetupParticle();
        }
        public void OnObjectSpawn() { }
        public void OnRespawn() { }

        private void SetupParticle() {
            _particleSystem = GetComponent<ParticleSystem>();
            if (_particleSystem.isPlaying)
                _particleSystem.Stop();
            _particleSystems = transform.GetComponentsInParent<ParticleSystem>().ToList();
        }

        public B_PooledParticle PlayParticle() {
            CancelOrder();
            ResetParticle();
            _playEffectCoroutine = IE_PlayEffect().RunCoroutine();
            return this;
        }
        
        public B_PooledParticle StopParticle() {
            if (_isLocked) return this;
            _isStopGiven = true;
            CancelOrder();
            return this;
        }
        
        public B_PooledParticle StopParticleImmediate() {
            _isStopGiven = true;
            CancelOrder();
            return this;
        }

        public B_PooledParticle LockParticle() {
            _isLocked = true;
            return this;
        }
        
        public B_PooledParticle UnlockParticle() {
            _isLocked = false;
            return this;
        }

        public B_PooledParticle SetDelay(float delay) {
            if(_isLocked) return this;
            CancelOrder();
            _playEffectCoroutine = IE_PlayEffect(delay).RunCoroutine();
            return this;
        }
        
        public B_PooledParticle SetLoop(int loop) {
            if(_isLocked) return this;
            CancelOrder();
            _playEffectCoroutine = IE_PlayEffect(loop).RunCoroutine();
            return this;
        }
        
        public B_PooledParticle SetLoop(int loop, float delay) {
            if(_isLocked) return this;
            CancelOrder();
            _playEffectCoroutine = IE_PlayEffect(loop, delay).RunCoroutine();
            return this;
        }
        
        public B_PooledParticle SetLoop(int loop, float delay, float initialDelay) {
            if(_isLocked) return this;
            CancelOrder();
            _playEffectCoroutine = IE_PlayEffect(loop, delay, initialDelay).RunCoroutine();
            return this;
        }
        
        private void CancelOrder() {
            _playEffectCoroutine?.StopCoroutine();
            _particleSystem.Clear();
            _particleSystem.Stop();
            _isPlaying = false;
            _isLocked = false;
            _isLooping = false;
            _isStopGiven = false;
        }

        public void ResetParticle() {
            _particleSystem.Clear();
            _particleSystem.Stop();
            _particleSystems.ForEach(t => t.Clear());
            _particleSystems.ForEach(t => t.Stop());
        }
        
        #region Ienumerators

        private IEnumerator IE_PlayEffect() {
            _particleSystem.Play();
            _isPlaying = true;
            yield return new WaitForSeconds(_particleSystem.main.duration);
            _isPlaying = false;
            _particleSystem.Clear();
            _particleSystem.Stop();
            OnComplete?.Invoke();
            OnComplete = null;
            _playEffectCoroutine = null;
        }
        
        private IEnumerator IE_PlayEffect(float delay) {
            yield return new WaitForSeconds(delay);
            _particleSystem.Play();
            _isPlaying = true;
            yield return new WaitForSeconds(_particleSystem.main.duration);
            _isPlaying = false;
            _particleSystem.Clear();
            _particleSystem.Stop();
            OnComplete?.Invoke();
            OnComplete = null;
        }

        private IEnumerator IE_PlayEffect(int Loop, [CanBeNull] float delayBetween = 0f, [CanBeNull] float delay = 0f) {
            yield return new WaitForSeconds(delay);
            if (Loop <= 0) {
                yield return IE_InfiniteLoop(delayBetween);
            }
            for (int i = 0; i < Loop; i++) {
                _particleSystem.Clear();
                yield return new WaitForFixedUpdate();
                _particleSystem.Play();
                yield return new WaitForSeconds(_particleSystem.main.duration + delayBetween);
                if (_isStopGiven) {
                    _isStopGiven = false;
                    StopParticleImmediate();
                    break;
                }
            }
            _isPlaying = false;
            _isLooping = false;
            OnComplete?.Invoke();
            _playEffectCoroutine = null;
        }

        private IEnumerator IE_InfiniteLoop([CanBeNull]float delayBetween = 0f) {
            _loop = 0;
            while (true) {

                _particleSystem.Clear();
                yield return new WaitForFixedUpdate();
                _particleSystem.Play();
                yield return new WaitForSeconds(_particleSystem.main.duration + delayBetween);
                yield return new WaitForSeconds(.01f);

                if (_isStopGiven) {
                    _isStopGiven = false;
                    StopParticleImmediate();
                }
            }
        }
        
        #endregion
    }
}