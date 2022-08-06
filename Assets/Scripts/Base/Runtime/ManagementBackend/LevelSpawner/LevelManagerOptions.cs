using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;

namespace Base {
    [Serializable]
    public class LevelManagerOptions {
        public List<LevelOptions> Levels;
        List<B_LevelPreparator> levelPreparators = new List<B_LevelPreparator>();

        bool _isInitialized;
        
        [Button]
        void SetupLevelManagerOptions() {
            levelPreparators = new List<B_LevelPreparator>();
            levelPreparators = Resources.LoadAll<B_LevelPreparator>(B_Database_String.Path_Res_MainLevels).ToList();
            Levels = new List<LevelOptions>();
            foreach (B_LevelPreparator level in levelPreparators) {
                Levels.Add(new LevelOptions(level.gameObject));
            }
        }
        
        #if Unity_Editor

        [Button]
        void Save() {
            if(Levels.IsNullOrEmpty()) return;
            Levels.ForEach(t => t.SaveSettings());
        }
        
        #endif
    }

    [Serializable]
    public class LevelOptions {
        private B_LevelPreparator preparator;
        public string LevelName;
        public GameObject LevelPrefab;
        [DisableIf("@true")]
        public string levelPath;
        public LevelOptions(GameObject levelPreparator) {
            levelPreparator.TryGetComponent(out B_LevelPreparator p);
            this.preparator = p;
            this.LevelName = levelPreparator.name;
            this.LevelPrefab = levelPreparator;
            this.levelPath = "Assets/Resources/Levels/MainLevels/" + this.LevelName + ".prefab";
        }
        
        #if Unity_Editor

        public void SaveSettings() {
            AssetDatabase.RenameAsset(levelPath, LevelName);
            AssetDatabase.SaveAssets();
        }

        #endif  
    }
}