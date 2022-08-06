using System;
using UnityEngine;
namespace Base.GameData {
    [Serializable]
    public class Data_Player {

        [Header("Player Settings")]
        public float Data_MaxHealth;
        public float Data_MovementSpeed;
        public float Data_JumpForce;
        [Header("Player Data")]
        public float Data_HighScore;
        public float Data_TotalCoin;

    }
    
}