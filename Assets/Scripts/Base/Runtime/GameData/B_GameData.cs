using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Base;
using Base.GameData;
using Base.UI;

public static class B_GameData {
    
    private static B_Manager_GameData _managerGameDataFunctions;
    
    public static Data_Player Data_Player => _managerGameDataFunctions.Data.Data_Player;
    public static Data_RemoteConfig Data_RemoteConfig => _managerGameDataFunctions.Data.Data_RemoteConfig;
    public static float Data_MaxHealth {
        get => _managerGameDataFunctions.Data.Data_Player.Data_MaxHealth;
        set => _managerGameDataFunctions.Data.Data_Player.Data_MaxHealth = value;
    }

    public static float Data_Speed {
        get => _managerGameDataFunctions.Data.Data_Player.Data_MovementSpeed;
        set => _managerGameDataFunctions.Data.Data_Player.Data_MovementSpeed = value;
    }
    
    public static float Data_JumpForce {
        get => _managerGameDataFunctions.Data.Data_Player.Data_JumpForce;
        set => _managerGameDataFunctions.Data.Data_Player.Data_JumpForce = value;
    }
    
    public static float Data_HighScore {
        get => _managerGameDataFunctions.Data.Data_Player.Data_HighScore;
        set => _managerGameDataFunctions.Data.Data_Player.Data_HighScore = value;
    }



    public static async Task Setup(B_Manager_GameData managerGameData) {
        
        _managerGameDataFunctions = managerGameData;
        await _managerGameDataFunctions.ManagerStrapping();
        // await Task.Delay(10000);
        B_CentralEventSystem.OnAfterLevelDisablePositive.AddFunction(OnLevelEnded, true);
        
    }
    
    public static void Player_HighScore_Set(float value) {

        if (value > _managerGameDataFunctions.Data.Data_Player.Data_HighScore) {
            _managerGameDataFunctions.Data.Data_Player.Data_HighScore = value;
            Enum_Menu_PlayerOverlayComponent.P_TXT_Player_HighScore.GetText().ChangeText(value.ToString("0"));
        }
        
    }
    
    public static void Player_CoinTotal_Set(float _coinTotal) {
        
        _managerGameDataFunctions.Data.Data_Player.Data_TotalCoin = _coinTotal;
        Enum_Menu_PlayerOverlayComponent.P_TXT_Player_TotalCoin.GetText().ChangeText(_managerGameDataFunctions.Data.Data_Player.Data_TotalCoin.ToString("0"));
    }
    
    public static void Player_CoinTotal_Add(float _coinTotal) {
        
        _managerGameDataFunctions.Data.Data_Player.Data_TotalCoin += _coinTotal;
        Enum_Menu_PlayerOverlayComponent.P_TXT_Player_TotalCoin.GetText().ChangeText(_managerGameDataFunctions.Data.Data_Player.Data_TotalCoin.ToString("0"));
        
    }
    
    private static void OnLevelEnded() {
        
    }
    
}