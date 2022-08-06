using System;
using System.Collections;
using System.Collections.Generic;
using Base;
using UnityEngine;


public abstract class StaticInstance<T> : MonoBehaviour where T : MonoBehaviour {
    public static T instance { get; private set; }
    protected virtual void Awake() {
        instance = this as T;
    }

    protected virtual void DestroyObject() {
        Destroy(gameObject);
    }
    
    protected virtual void NullOut() {
        instance = null;
    }
    
    protected virtual void OnApplicationPause(bool pauseStatus) {
        
    }
    protected virtual void OnApplicationQuit() {
        NullOut();
        DestroyObject();
    }


}

public abstract class Singleton<T> : StaticInstance<T> where T : MonoBehaviour {
    protected override void Awake() {
        if (instance != null) { base.DestroyObject(); return;}
        base.Awake();
    }

    protected void OnDisable() {
        base.NullOut();
    }
}

public abstract class PersistentSingleton<T> : Singleton<T> where T : MonoBehaviour {
    protected override void Awake() {
        if(instance != null) { base.DestroyObject(); return;}
        base.Awake();
    }
}