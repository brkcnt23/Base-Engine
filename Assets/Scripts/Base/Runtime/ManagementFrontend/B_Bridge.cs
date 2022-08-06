using System;
using System.Threading.Tasks;
using Base.SoundManagement;
using Base.UI;
using Sirenix.OdinInspector;
using UnityEngine;
namespace Base {
    [Serializable]
    public class B_Bridge {
        
        #region Managers
        
        [TabGroup("FirstGroup", "Game Manager")]
        [HideLabel]
        public GameManagerFunctions gameManagerFunctions;
        [TabGroup("FirstGroup", "UI Manager")]
        [HideLabel]
        public UIManagerFunctions UIManager;
        [TabGroup("FirstGroup", "Level Manager")]
        [HideLabel]
        public LevelManagerFunctions levelManagerFunctions;
        [TabGroup("SecondGroup", "Coroutine Manager")]
        [HideLabel]
        public CoroutineRunnerFunctions coroutineRunnerFunctions;
        [TabGroup("SecondGroup", "Camera Manager")]
        [HideLabel]
        public CameraFunctions CameraFunctions;
        [TabGroup("SecondGroup","Audio Manager")]
        [HideLabel]
        public SoundManager AudioFunctions;
        [TabGroup("ThirdGroup", "Player Data")]
        [HideLabel]
        public B_Manager_GameData ManagerGameData;
        
        [HideInInspector] public bool Initialized = false;
        #endregion

        public async Task SetupBridge(BaseEngine bootLoader) {

            B_UIControl.Setup(UIManager);
            await B_GameData.Setup(ManagerGameData);
            B_CoroutineControl.Setup(bootLoader, coroutineRunnerFunctions);
            B_GameControl.Setup(gameManagerFunctions);
            B_LevelControl.Setup(levelManagerFunctions);
            B_CameraControl.Setup(CameraFunctions);
            SoundControl.Setup(AudioFunctions);
            GameObject.FindObjectOfType<SoundSettings>().Activate();
            // await Task.Delay(1000);
            Initialized = true;

        }
        
        #region Control Functions

        public void FlushBridgeData() {
            
        }

        #endregion
        
    }
}