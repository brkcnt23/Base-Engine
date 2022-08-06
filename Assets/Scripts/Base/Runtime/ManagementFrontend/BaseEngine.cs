using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Base.UI;
using DG.Tweening;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
#if UNITY_EDITOR
#endif
#if UNITY_IOS
using Unity.Advertisement.IosSupport;
#endif

namespace Base {

    [DefaultExecutionOrder(-100)]
    public class BaseEngine : MonoBehaviour {

        #region Properties
        
        [TabGroup("Master", "Bridge")]
        [HideLabel]
        public B_Bridge Bridge;

        [TabGroup("Master", "Game Aspect Control")]
        [SerializeField] private bool HasTutorial;
        [TabGroup("Master", "Editor Control")]
        [TabGroup("Master", "Editor Control")]
        [SerializeField] private B_EffectsFunctions effectsFunctions;
        [TabGroup("Master", "Editor Control")]
        [SerializeField] private bool ShowFps;
        /// <summary>
        /// Set to -1 to get unlimited frame rate
        /// </summary>
        [TabGroup("Master", "Game Aspect Control")]
        [SerializeField] private int TargetFrameRate = 60;
        [TabGroup("Master", "Game Aspect Control")]
        [SerializeField] private bool TestMode;
        private bool _isInitialized;

        #endregion
        
        #region Unity Functions

        private void Awake() {
            StartCoroutine(Preloader());
        }

        private IEnumerator Preloader() {
            
            //If you need to setup data tracking, ask for permissions here
#if UNITY_IOS
            //Asks for permission on iOS devices
            if (ATTrackingStatusBinding.GetAuthorizationTrackingStatus() == ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED || ATTrackingStatusBinding.GetAuthorizationTrackingStatus() == ATTrackingStatusBinding.AuthorizationTrackingStatus.RESTRICTED || ATTrackingStatusBinding.GetAuthorizationTrackingStatus() == ATTrackingStatusBinding.AuthorizationTrackingStatus.DENIED)
            {
                yield return new WaitForSeconds(1.2f);
                ATTrackingStatusBinding.RequestAuthorizationTracking();
            }
#else
            yield return null;
#endif
            SRDebug.Init();
            StartCoroutine(InitiateBootLoading());
        }

        #endregion

        #region Spesific Functions


        /// <summary>
        /// Prepares every aspect of the Base Engine
        /// Loads and sets up managers, level loading system, save system etc.
        /// </summary>
        private IEnumerator InitiateBootLoading() {
#if UNITY_EDITOR
            Debug.unityLogger.logEnabled = TestMode;
#else
            Debug.unityLogger.logEnabled = TestMode;   
#endif
            yield return new WaitUntil(B_CentralEventSystem.CentralEventSystemStrapping);
            Bridge.SetupBridge(this);
            yield return new WaitUntil(() => Bridge.Initialized);

            if (!HasTutorial) Enum_MainSave.Game_TutorialPlayed.SetData(1);
            yield return new WaitUntil(effectsFunctions.VFXManagerStrapping);
            yield return new WaitUntil(B_EffectsManager.EffectsManagerStrapping);

            GameStates.Start.SetGameState();
            
            B_GameControl.SaveAllGameData();

            yield return new WaitForSeconds(.1f);

            if (TargetFrameRate < 0) {
                TargetFrameRate = -1;
            }
            Application.targetFrameRate = TargetFrameRate;
            
            yield return new WaitUntil(CalculateTime);
            B_LevelControl.LoadLevel(Enum_MainSave.Player_RealLevel.ToInt());

            Bridge.gameManagerFunctions.IsInitialized = true;
            _isInitialized = true;
        }
        private void OnApplicationQuit() {
            if(!_isInitialized) return;
            B_GameControl.SaveAllGameData();
            DOTween.KillAll();
        }


        private void OnApplicationPause(bool pauseStatus) {
            if(!_isInitialized) return;
            #if UNITY_IOS
            B_GameControl.SaveAllGameData();
            
            DOTween.KillAll();
            #endif
        }
        private float deltaTime;
        
        void Update()
        {
            if(ShowFps)
                deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        }

        void OnGUI()
        {
            if(ShowFps)
                ShowFPS();
        }

        private void ShowFPS()
        {
            int w = Screen.width, h = Screen.height;

            GUIStyle style = new GUIStyle();

            Rect rect = new Rect(0, 0, w, h * 2 / 100);
            style.alignment = TextAnchor.UpperLeft;
            style.fontSize = h * 2 / 100;
            style.normal.textColor = Color.black;
            float msec = deltaTime * 1000.0f;
            float fps = 1.0f / deltaTime;
            string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
            GUI.Label(rect, text, style);
        }

        private bool CalculateTime() {
            DateTime todayLogin = DateTime.Now;
            if (Enum_MainSave.Date_LastLogin.GetData().ToString().IsNullOrWhitespace() || Enum_MainSave.Date_LastLogin.GetData().ToString() == "") {
                
                Enum_MainSave.Date_LastLogin.SetData(todayLogin.ToString());
                
                Enum_MainSave.Date_TimeSinceLastLogin_Days.SetData(0);
                Enum_MainSave.Date_TimeSinceLastLogin_Hours.SetData(0);
                Enum_MainSave.Date_TimeSinceLastLogin_Minutes.SetData(0);
                Enum_MainSave.Date_TimeSinceLastLogin_Seconds.SetData(0);
                return true;
            }
            DateTime lastLogin = DateTime.Parse(Enum_MainSave.Date_LastLogin.GetData().ToString());
            // DateTime lastLogin = new DateTime();
            //Take the difference in days between the last login and today
            int daysSinceLastLogin = (todayLogin - lastLogin).Days;
            //Take the difference in hours between the last login and today
            int hoursSinceLastLogin = (todayLogin - lastLogin).Hours;
            //Take the difference in minutes between the last login and today
            int minutesSinceLastLogin = (todayLogin - lastLogin).Minutes;
            //Take the difference in seconds between the last login and today
            int secondsSinceLastLogin = (todayLogin - lastLogin).Seconds;
            
            float totalTimeInDays = daysSinceLastLogin + hoursSinceLastLogin / 24f + minutesSinceLastLogin / 1440f + secondsSinceLastLogin / 86400f;
            float totalTimeInHours = totalTimeInDays * 24f;
            float totalTimeInMinutes = totalTimeInHours * 60f;
            float totalTimeInSeconds = totalTimeInMinutes * 60f;
            
            
            Enum_MainSave.Date_TimeSinceLastLogin_Days.SetData(totalTimeInDays);
            Enum_MainSave.Date_TimeSinceLastLogin_Hours.SetData(totalTimeInHours);
            Enum_MainSave.Date_TimeSinceLastLogin_Minutes.SetData(totalTimeInMinutes);
            Enum_MainSave.Date_TimeSinceLastLogin_Seconds.SetData(totalTimeInSeconds);
                
            Enum_MainSave.Date_LastLogin.SetData(DateTime.Now.ToString());

            return true;
        }

        #endregion

    }

    public enum GameStartType {
        Button,
        Auto,
        
    }
}