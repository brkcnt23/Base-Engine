using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Base.Plugins.Unlockable {
    public enum P_Unlockable_Status {
        Hidden,
        Uninteractable,
        Interactable,
        Bought
    }
    [Serializable]
    public class P_Unlockable_Data {

        [BoxGroup("General")]
        public P_Unlockable_Status Status;
        [BoxGroup("General")]
        public int Cost;
        [BoxGroup("Idle")]
        public bool HasIncome = false;

        [ShowIf("@HasIncome")]
        [BoxGroup("Idle")] 
        public float IncomePerSecond;
    }
    
}