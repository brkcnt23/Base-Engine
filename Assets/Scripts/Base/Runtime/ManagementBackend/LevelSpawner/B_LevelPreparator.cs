using System;
using Sirenix.OdinInspector;
using UnityEngine;
namespace Base {
    public class B_LevelPreparator : MonoBehaviour {

        
        private int levelCount;

        private void Awake() {
            B_CentralEventSystem.OnAfterLevelLoaded.AddFunction(OnLevelInitate, false);
            B_CentralEventSystem.OnLevelActivation.AddFunction(OnLevelCommand, false);
        }

        private void OnDisable() {
            B_CentralEventSystem.OnLevelDisable.InvokeEvent();
        }

        public void OnLevelInitate() {
            //GameManagerFunctions.instance.SaveSystem.PlayerLevel = levelCount;
            B_SaveSystem.SetData(Enum_MainSave.Player_RealLevel, levelCount);
            Debug.Log("Level Loaded");
        }

        public void OnLevelCommand() {
            Debug.Log("Level Started");
        }
    }
}