using System;
using Base;
using UnityEngine;
namespace Testing.Idle {
    public class IdleTimer : MonoBehaviour {

        public GameObject obj;
        private float _coinPerSecond = 2;
        private void Start() {
            CalculateTotalCoins();
        }
        
        private void CalculateTotalCoins() {
            float seconds = Enum_MainSave.Date_TimeSinceLastLogin_Seconds.ToFloat();
            float totalCoins = seconds * _coinPerSecond;
            Debug.Log($"{seconds} seconds has passed since last login. {totalCoins} coins have been earned.");
            Debug.Log($"Game Language: {B_GameData.Data_RemoteConfig.GameLanguage} / Game Version: {B_GameData.Data_RemoteConfig.GameVersion}");
            ColorUtility.TryParseHtmlString (B_GameData.Data_RemoteConfig.PlayerColor, out Color MyColour);
            obj.GetComponent<Renderer>().material.color = MyColour;
        }
    }
}