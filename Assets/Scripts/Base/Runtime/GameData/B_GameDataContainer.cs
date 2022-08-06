using System.Collections;
using System.Collections.Generic;
using Base.GameData;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "Player_Container", menuName = "Base/Player")]
[InlineEditor]
public class B_GameDataContainer : ScriptableObject {

    public Data_Player Data_Player;
    public Data_RemoteConfig Data_RemoteConfig = new Data_RemoteConfig();


}



