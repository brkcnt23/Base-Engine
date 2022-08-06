using System.Collections.Generic;
using Base.UI;
using UnityEngine;
namespace Base {
    public static class B_LevelControl {
        
        private static LevelManagerFunctions _levelManagerFunctions;
        public static Transform CurrentLevelObject => _levelManagerFunctions.LevelHolder;
        public static void Setup(LevelManagerFunctions levelManagerFunctions) {
            _levelManagerFunctions = levelManagerFunctions;
            _levelManagerFunctions.ManagerStrapping();
        }

        public static void LoadLevel(int index) => _levelManagerFunctions.LoadInLevel(index);

        public static void LoadNextLevel() => _levelManagerFunctions.LoadInNextLevel();

        public static void RestartLevel() => _levelManagerFunctions.ReloadCurrentLevel();
        
    }
}