using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Base {
    public class CoroutineQueue {
        private readonly Queue<IEnumerator> _actions = new Queue<IEnumerator>();
        private Coroutine _internalCoroutine;
        private readonly MonoBehaviour _owner;

        public CoroutineQueue(MonoBehaviour aCoroutineOwner) {
            _owner = aCoroutineOwner;
        }

        public void StartLoop() {
            _internalCoroutine = _owner.StartCoroutine(Process());
        }

        public void StopLoop() {
            _owner.StopCoroutine(_internalCoroutine);
            _internalCoroutine = null;
        }

        public void EnqueueAction(IEnumerator aAction) {
            _actions.Enqueue(aAction);
        }

        public void EnqueueWait(float aWaitTime) {
            _actions.Enqueue(Wait(aWaitTime));
        }

        private IEnumerator Wait(float aWaitTime) {
            yield return new WaitForSeconds(aWaitTime);
        }

        private IEnumerator Process() {
            while (true)
                if (_actions.Count > 0)
                    yield return _owner.StartCoroutine(_actions.Dequeue());
                else
                    yield return null;
        }

        public Coroutine RunCoroutine(IEnumerator enumerator) {
            return _owner.StartCoroutine(enumerator);
        }

        public Coroutine RunCoroutine(IEnumerator enumerator, float delay) {
            return _owner.StartCoroutine(Ienum_DelayStartIenum(enumerator, delay));
        }

        public void RunCoroutine(IEnumerator enumerator, Coroutine coroutine) {
            if (coroutine == null) {
                coroutine = _owner.StartCoroutine(enumerator);
            }
            else {
                _owner.StopCoroutine(coroutine);
                coroutine = null;
                coroutine = _owner.StartCoroutine(enumerator);
            }
        }

        public void RunCoroutine(IEnumerator enumator, Coroutine coroutine, float waitTime) {
            _owner.StartCoroutine(Ienum_DelayStartIenum(enumator, coroutine, waitTime));
        }

        private IEnumerator Ienum_DelayStartIenum(IEnumerator enumerator, Coroutine coroutine, float waitTime) {
            yield return new WaitForSeconds(waitTime);
            if (coroutine == null) {
                coroutine = _owner.StartCoroutine(enumerator);
            }
            else {
                _owner.StopCoroutine(coroutine);
                coroutine = null;
                coroutine = _owner.StartCoroutine(enumerator);
            }
        }

        private IEnumerator Ienum_DelayStartIenum(IEnumerator enumerator, float waitTime) {
            yield return new WaitForSeconds(waitTime);
            _owner.StartCoroutine(enumerator);
        }

        public Coroutine RunFunctionWithDelay(Action method, float waitTime) {
            return _owner.StartCoroutine(Ienum_DelayStartFunction(method, waitTime));
        }

        private IEnumerator Ienum_DelayStartFunction(Action method, float waitTime) {
            yield return new WaitForSeconds(waitTime);
            method?.Invoke();
        }
        
        public Coroutine StopCoroutine(Coroutine coroutine) {
            if (coroutine != null) {
                _owner.StopCoroutine(coroutine);
                coroutine = null;
            }
            return null;
        }
    }
}