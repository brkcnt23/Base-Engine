using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Base.UI;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;
namespace Base {
    public enum LevelType { Tutorial, Main }
    [Serializable]
    public class LevelManagerFunctions : B_ManagerBase {
        
        [Header("Indiviual Level Settings")]
        [HideLabel]
        public LevelManagerOptions _levelManagerOptions;
        
        public static Transform ObjectSpawnParent;

        [HideInInspector] public List<GameObject> MainLevels;
        [HideInInspector] public List<GameObject> TutorialLevels;

        [HideInInspector] public GameObject CurrentLevel;

        [HideInInspector] public int CurrentLevelIndex;
        [HideInInspector] public int PreviewLevelIndex;
        private GameObject currentLevel;

        public Action<int> OnLevelChangedAction;

        [HideInInspector] public Transform LevelHolder { get; private set; }

        private int tutorialPlayed => Enum_MainSave.Game_TutorialPlayed.ToInt();

        private void OnDestroy() {
            ObjectSpawnParent = null;
        }

        public override Task ManagerStrapping() {

            LevelHolder = GameObject.Find("LevelHolder").GetComponent<Transform>();

            MainLevels = new List<GameObject>();
            TutorialLevels = new List<GameObject>();

            MainLevels = Resources.LoadAll<GameObject>(B_Database_String.Path_Res_MainLevels).ToList();
            TutorialLevels = Resources.LoadAll<GameObject>(B_Database_String.Path_Res_TutorialLevels).ToList();

            MainLevels = MainLevels.OrderBy(t => t.name).ToList();
            TutorialLevels = TutorialLevels.OrderBy(t => t.name).ToList();

            PreviewLevelIndex = Enum_MainSave.Player_PreviewLevel.ToInt();
            
            B_CentralEventSystem.OnBeforeLevelDisablePositive.AddFunction(SaveOnNextLevel, true);

            return base.ManagerStrapping();
        }

        public override Task ManagerDataFlush() {
            return base.ManagerDataFlush();
        }

        public void LoadInLevel(int levelNumber) {
            switch (tutorialPlayed) {
                case 0:
                    if (levelNumber >= TutorialLevels.Count) levelNumber = 0;
                    InitateNewLevel(TutorialLevels[levelNumber]).RunCoroutine();
                    break;

                case 1:
                    if (levelNumber >= MainLevels.Count) levelNumber = 0;
                    InitateNewLevel(MainLevels[levelNumber]).RunCoroutine();
                    break;
            }
        }

        public void LoadInNextLevel() {
            switch (Enum_MainSave.Game_Finished.ToInt()) {
                case 0:
                    InitateNewLevel(LevelToLoad()).RunCoroutine();
                    break;

                case 1:
                    InitateNewLevel(RandomSelectedLevel()).RunCoroutine();
                    break;
            }
        }

        public void ReloadCurrentLevel() {
            
            InitateNewLevel(currentLevel).RunCoroutine();
        }

        private IEnumerator InitateNewLevel(GameObject levelToInit) {
            B_GUIManager.ActivateOnePanel(Enum_MenuTypes.Menu_Loading);
            B_CentralEventSystem.OnBeforeLevelLoaded.InvokeEvent();
            if (CurrentLevel != null) {
                GameObject.Destroy(CurrentLevel);
                CurrentLevel = null;
                currentLevel = null;
            }

            yield return new WaitForSeconds(.1f);
            
            CurrentLevel = GameObject.Instantiate(levelToInit, LevelHolder);
            currentLevel = levelToInit;
            ObjectSpawnParent = CurrentLevel.transform;
            switch (tutorialPlayed) {
                case 0:
                    CurrentLevelIndex = Array.IndexOf(TutorialLevels.ToArray(), levelToInit);
                    B_SaveSystem.SetData(Enum_MainSave.Player_RealLevel, CurrentLevelIndex);
                    B_SaveSystem.SetData(Enum_MainSave.Player_PreviewLevel, CurrentLevelIndex);
                    OnLevelChangedAction?.Invoke(CurrentLevelIndex);
                    break;

                case 1:
                    CurrentLevelIndex = Array.IndexOf(MainLevels.ToArray(), levelToInit);
                    B_SaveSystem.SetData(Enum_MainSave.Player_RealLevel, CurrentLevelIndex);
                    OnLevelChangedAction?.Invoke(PreviewLevelIndex);
                    break;
            }
            B_CentralEventSystem.OnAfterLevelLoaded.InvokeEvent();
            B_SaveSystem.SetData(Enum_MainSave.Player_RealLevel, CurrentLevelIndex);
            
            switch (B_GameControl.CurrentGameStartType) {
                case GameStartType.Button:
                    B_GUIManager.ActivateOnePanel(Enum_MenuTypes.Menu_Main);
                    break;
                case GameStartType.Auto:
                    B_GUIManager.ActivateOnePanel(Enum_MenuTypes.Menu_PlayerOverlay);
                    GameStates.Playing.SetGameState();
                    break;
            }
        }

        private GameObject LevelToLoad() {
            switch (tutorialPlayed) {
                case 0:
                    if (CurrentLevelIndex + 1 >= TutorialLevels.Count) {
                        CurrentLevelIndex = 0;
                        B_SaveSystem.SetData(Enum_MainSave.Game_TutorialPlayed, 1);
                        return MainLevels[0];
                    }
                    return TutorialLevels[CurrentLevelIndex + 1];

                case 1:
                    if (CurrentLevelIndex + 1 >= MainLevels.Count) {
                        B_SaveSystem.SetData(Enum_MainSave.Game_Finished, 1);
                        return RandomSelectedLevel();
                    }
                    return MainLevels[CurrentLevelIndex + 1];
            }
            return null;
        }

        private void CheckLevels() {
            for (var i = 0; i < MainLevels.Count; i++) Debug.Log(MainLevels[i].name);
            Debug.Log("//----------//");
            for (var i = 0; i < TutorialLevels.Count; i++) Debug.Log(TutorialLevels[i].name);
        }

        private GameObject RandomSelectedLevel() {
            if (MainLevels.Count <= 1) return MainLevels[0];
            var obj = MainLevels[Random.Range(0, MainLevels.Count)];
            if (currentLevel == obj) return RandomSelectedLevel();
            return obj;
        }

        private void SaveOnNextLevel() {
            B_SaveSystem.SetData(Enum_MainSave.Player_PreviewLevel, PreviewLevelIndex + 1);
        }
    }
}