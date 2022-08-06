using System;
using System.Threading.Tasks;
using Base.UI;
using UnityEngine;
namespace Base {
    public enum GameStates { Init, Start, Paused, Playing, End }

    [Serializable]
    public class GameManagerFunctions : B_ManagerBase {
        
        // //Fires when the game state changes
        public static Action OnGameStateChange;
        //Stores the actual game state
        private GameStates _currentGameState;
        //Controls the save system
        public B_SaveSystemEditor SaveSystem;
        //Activates Runtime Editor
        public bool ActivateRuntimeEditor;
        // private RuntimeEditor _runtimeEditor;
        
        [SerializeField] public GameStartType GameStartType;
        [HideInInspector] public bool IsInitialized;
        
        public GameStates CurrentGameState {
            get => _currentGameState;
            set {
                if (_currentGameState == value) return;
                _currentGameState = value;
                B_CentralEventSystem.OnGameStateChange.InvokeEvent();
            }
        }

        public override Task ManagerStrapping() {
            SaveSystem = new B_SaveSystemEditor();
            
            Enum_Menu_MainComponent.BTN_Start.GetButton().AddFunction(StartGame);
            Enum_Menu_GameOverComponent.BTN_Sucess.GetButton().AddFunction(EndLevel);
            Enum_Menu_GameOverComponent.BTN_Fail.GetButton().AddFunction(RestartLevel);
            
            return base.ManagerStrapping();
        }
        public override Task ManagerDataFlush() {
            return base.ManagerDataFlush();
        }

        public bool IsGamePlaying() {
            if (CurrentGameState == GameStates.Playing) return true;
            return false;
        }

        #region Game Management Functions

        /// <summary>
        /// Starts the game changing the UI and setting the game state to GameStates.Playing
        /// </summary>
        private void StartGame() {
            B_CentralEventSystem.BTN_OnStartPressed.InvokeEvent();
            CurrentGameState = GameStates.Playing;
            B_GUIManager.ActivateOnePanel(Enum_MenuTypes.Menu_PlayerOverlay);
        }

        /// <summary>
        /// Reloads the game without saving any data
        /// </summary>
        private void RestartLevel() {
            B_CentralEventSystem.BTN_OnRestartPressed.InvokeEvent();
            B_LevelControl.RestartLevel();
            B_GUIManager.ActivateOnePanel(Enum_MenuTypes.Menu_Main, .3f);
        }
        /// <summary>
        /// Ends the level on press of a button
        /// </summary>
        private void EndLevel() {
            B_CentralEventSystem.BTN_OnEndGamePressed.InvokeEvent();
            CurrentGameState = GameStates.Start;
            B_GUIManager.ActivateOnePanel(Enum_MenuTypes.Menu_Main);
            B_LevelControl.LoadNextLevel();
        }

        /// <summary>
        /// Activates the end game, changes game state and fires signals
        /// </summary>
        /// <param name="Success"></param>
        /// Set true for if player won, false if player lost
        /// <param name="Delay"></param>
        /// Set time for the delay on UI. Doesn't effects signals
        public async void ActivateEndgame(bool Success, float Delay = 0) {
            if (CurrentGameState == GameStates.End || CurrentGameState == GameStates.Start) return;
            B_CentralEventSystem.OnBeforeLevelEnd.InvokeEvent();
            CurrentGameState = GameStates.End;
            switch (Success) {
                case true:
                    B_CentralEventSystem.OnBeforeLevelDisablePositive.InvokeEvent();
                    break;
                case false:
                    B_CentralEventSystem.OnBeforeLevelDisableNegative.InvokeEvent();
                    break;
            }
            await Task.Delay((int)Delay * 1000);
            B_GUIManager.ActivateOnePanel(Enum_MenuTypes.Menu_GameOver);
            B_GUIManager.GameOver.EnableOverUI(Success);
            SaveSystem.SaveAllData();
            B_CentralEventSystem.OnAfterLevelEnd.InvokeEvent();
        }

        //Uncomment these functions if you want game to save data on pause or quit

        private void OnApplicationPause(bool pause) {
            SaveSystem.SaveAllData();
        }

        private void OnApplicationQuit() {
            SaveSystem.SaveAllData();
        }

        #endregion
    //
    //     #region Runtime Editor
    //
    //     private void OnGUI() {
    //         if(ActivateRuntimeEditor)
    //             _runtimeEditor.OnGUI();
    //     }
    //
    //     public class RuntimeEditor {
    // #if UNITY_EDITOR
    //
    //         private GUIStyle myButtonStyle;
    //         private Rect turnButton;
    //         private GUIStyle mainTitleText, titleText;
    //         private string turnStat = "X";
    //         private bool time = true;
    //         private string timeText = "STOP GAME";
    //         private string gameStat = "ON START";
    //
    //         public void Setup() {
    //             turnButton = new Rect(900, 275, 200, 50);
    //             mainTitleText = new GUIStyle();
    //             mainTitleText.fontSize = 36;
    //             mainTitleText.normal.textColor = Color.red;
    //             titleText = new GUIStyle();
    //             titleText.fontSize = 24;
    //             titleText.normal.textColor = Color.white;
    //         }
    //
    //         private bool open = true;
    //         
    //         public void OnGUI() {
    //             if (open) {
    //                 myButtonStyle = new GUIStyle(GUI.skin.button);
    //                 myButtonStyle.fontSize = 30;
    //
    //                 GUI.Box(new Rect(0, 100, Screen.width, Screen.height / 8), "");
    //
    //                 GUI.Label(new Rect(0, 100, 200, 50), "MENU", mainTitleText);
    //                 GUI.Label(new Rect(0, 135, 200, 50), "CHANGE GAME STAT", titleText);
    //
    //                 //They dont do anything
    //                 // if (GUI.Button(new Rect(0, 175, 200, 50), "On Start", myButtonStyle)) {
    //                 //     GameManagerFunctions.instance.CurrentGameState = GameStates.Start;
    //                 //     gameStat = "ON START";
    //                 // }
    //                 //
    //                 // if (GUI.Button(new Rect(225, 175, 200, 50), "On Playing", myButtonStyle)) {
    //                 //     GameManagerFunctions.instance.CurrentGameState = GameStates.Playing;
    //                 //     gameStat = "ON PLAYING";
    //                 // }
    //                 //
    //                 // if (GUI.Button(new Rect(450, 175, 200, 50), "On Pause", myButtonStyle)) {
    //                 //     GameManagerFunctions.instance.CurrentGameState = GameStates.Paused;
    //                 //     gameStat = "ON PAUSE";
    //                 // }
    //                 //
    //                 // if (GUI.Button(new Rect(675, 175, 200, 50), "On Init", myButtonStyle)) {
    //                 //     GameManagerFunctions.instance.CurrentGameState = GameStates.Init;
    //                 //     gameStat = "ON INIT";
    //                 // }
    //                 //
    //                 // if (GUI.Button(new Rect(900, 175, 200, 50), "On End", myButtonStyle)) {
    //                 //     GameManagerFunctions.instance.CurrentGameState = GameStates.End;
    //                 //     gameStat = "ON END";
    //                 // }
    //
    //                 GUI.Label(new Rect(0, 235, 200, 50), "CHANGE", titleText);
    //
    //                 if (GUI.Button(new Rect(0, 275, 200, 50), timeText, myButtonStyle)) {
    //                     time = !time;
    //
    //                     if (!time) {
    //                         timeText = "START GAME";
    //                         Time.timeScale = 0;
    //                     }
    //                     else {
    //                         Time.timeScale = 1;
    //                         timeText = "STOP GAME";
    //                     }
    //                 }
    //
    //                 if (GUI.Button(new Rect(225, 275, 200, 50), "RESTART LEVEL", myButtonStyle)) {
    //                     instance.RestartLevel();
    //                 }
    //
    //                 GUI.Label(new Rect(225, 235, 200, 50), "GAME STAT: " + gameStat, titleText);
    //
    //                 if (GUI.Button(new Rect(450, 275, 200, 50), "F. GAME(T)", myButtonStyle)) {
    //                     instance.ActivateEndgame(true);
    //                 }
    //
    //                 if (GUI.Button(new Rect(675, 275, 200, 50), "F. GAME(F)", myButtonStyle)) {
    //                     instance.ActivateEndgame(false);
    //                 }
    //             }
    //
    //             if (GUI.Button(turnButton, turnStat, myButtonStyle)) {
    //                 open = !open;
    //
    //                 if (!open) {
    //                     turnButton = new Rect(0, 0, 200, 50);
    //                     turnStat = "O";
    //                 }
    //                 else {
    //                     turnButton = new Rect(900, 275, 200, 50);
    //                     turnStat = "X";
    //                 }
    //             }
    //         }
    //
    // #endif
    //     }
    //
    //     #endregion
    //     
    }
}