using System;
using System.Threading.Tasks;
using Base;
using Base.GameData;
using Sirenix.OdinInspector;
using Unity.RemoteConfig;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
[Serializable]
public class B_Manager_GameData : B_ManagerBase {
    
    [HideLabel]
    public B_GameDataContainer Data;

    [BoxGroup("Game Data")]
    [SerializeField] private bool IsTesting = false;
    [BoxGroup("Game Data")]
    [SerializeField] private string TestEnvironment;
    [BoxGroup("Game Data")]
    [SerializeField] private string ProductionEnvironment;
    private string CurrentEnvironment => IsTesting ? TestEnvironment : ProductionEnvironment;
    public override async Task ManagerStrapping() {
        
        Data.Data_RemoteConfig = new Data_RemoteConfig();
        
        await InitializeRemoteConfigAsync();

        userAttributes uAStruct = new userAttributes();
        
        uAStruct.score = 10;
        
        ConfigManager.FetchConfigs<userAttributes, appAttributes>(uAStruct, new appAttributes(){});
        
        ConfigManager.SetEnvironmentID(CurrentEnvironment);
        
        ConfigManager.FetchCompleted += RemoteConfigLoaded;
    }

    public override Task ManagerDataFlush() {
        return base.ManagerDataFlush();
    }
    
    async Task InitializeRemoteConfigAsync() {
        
        await UnityServices.InitializeAsync();

        if (!AuthenticationService.Instance.IsSignedIn) {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }
    
    private void RemoteConfigLoaded(ConfigResponse response) {

        switch (response.requestOrigin) {

            case ConfigOrigin.Default:
                Debug.Log("No settings loaded, using default values.");
                break;
            case ConfigOrigin.Cached:
                Debug.Log("No settings loaded, using cached values.");
                break;
            case ConfigOrigin.Remote:
                Debug.Log(Data.Data_RemoteConfig);
                Data.Data_RemoteConfig.GameLanguage = ConfigManager.appConfig.GetString("gamelanguage");
                Data.Data_RemoteConfig.GameVersion = ConfigManager.appConfig.GetString("gameversion");
                Data.Data_RemoteConfig.PlayerColor = ConfigManager.appConfig.GetString("playercolor");
                
                break;
        }

    }

}

public struct userAttributes {
    public int score;
}

public struct appAttributes {
    
}