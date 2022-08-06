using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

[ExecuteAlways]
public class EditorCleanupSignaller : MonoBehaviour {
#if UNITY_EDITOR
    
    bool levelLoaded = false;
    string lastLevel {
        get => PlayerPrefs.GetString("EDITOR_LAST_LEVEL", "");
        set => PlayerPrefs.SetString("EDITOR_LAST_LEVEL", value);
    }
    void Update() {
        TryGetComponent(out B_LevelCreator levelCreator);
        if (!levelLoaded) {
            if(EditorApplication.isPlaying) return;
            levelLoaded = true;
            if(lastLevel.IsNullOrWhitespace()) return;
            levelCreator.EditorPlaytimeLoad(lastLevel);
        }
        if (EditorApplication.isPlayingOrWillChangePlaymode) {
            lastLevel = levelCreator.SelectedLevel;
            levelCreator.EditorPlaytimeClear();
            levelLoaded = false;
        }
    }
    
#endif
}