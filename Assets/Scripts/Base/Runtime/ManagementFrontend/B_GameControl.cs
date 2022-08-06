using System.Runtime.InteropServices;
using JetBrains.Annotations;
namespace Base {
    public static class B_GameControl {

        private static GameManagerFunctions _managerFunctionsMain;
        
        public static GameStates CurrentGameState => _managerFunctionsMain.CurrentGameState;
        public static GameStartType CurrentGameStartType => _managerFunctionsMain.GameStartType;
        public static B_SaveSystemEditor MainSaveSystemSystem => _managerFunctionsMain.SaveSystem;
        public static bool IsInitialized => _managerFunctionsMain.IsInitialized;

        public static void Setup(GameManagerFunctions gameManagerFunctions) {
            _managerFunctionsMain = gameManagerFunctions;
            _managerFunctionsMain.ManagerStrapping();
        }

        
        public static void SetGameState(this GameStates states) => _managerFunctionsMain.CurrentGameState = states;
        
        public static void SaveAllGameData() => _managerFunctionsMain.SaveSystem.SaveAllData();
        
        public static bool IsGamePlaying() => CurrentGameState == GameStates.Playing;

        public static void ActivateEndgame(bool success, [CanBeNull] float delay = 0) => _managerFunctionsMain.ActivateEndgame(success, delay);
        
    }
}