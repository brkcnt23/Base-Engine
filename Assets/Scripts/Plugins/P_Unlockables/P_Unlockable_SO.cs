using System;
using System.Collections;
using System.Collections.Generic;
using Base;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Base.Plugins.Unlockable {
    [CreateAssetMenu(fileName = "New Level Setting", menuName = "Unlockable/Level Setting")]
    public class P_Unlockable_SO : ScriptableObject {
        
        [BoxGroup("Main")] [SerializeField] protected AnimationCurve StationCostCurve;

        [BoxGroup("Main")] [MinMaxSlider(0f, 100f, true)] [SerializeField]
        protected Vector2 StationCostCurveRange;

        [BoxGroup("Main")] public int BaseCost;

        [TabGroup("Station")] public int CostRoundTo = 10;

        [TabGroup("Station")] public P_Unlockable_Data[] UnlockableDatas;

        public virtual void SetupUnlockables(P_Unlockable_Object[] unlockables, bool useMinMax = true) {
            if (useMinMax) SetupCurve();

            UnlockableDatas = new P_Unlockable_Data[unlockables.Length];

            for (var i = 0; i < unlockables.Length; i++) {
                float costMulti = i.Remap(0f, (float)unlockables.Length - 1, 0f, 1f);
                costMulti = StationCostCurve.Evaluate(costMulti);


                float incomeMulti = i.Remap(0f, (float)unlockables.Length - 1, 0f, 1f);
                incomeMulti = StationCostCurve.Evaluate(incomeMulti);

                UnlockableDatas[i] = new P_Unlockable_Data();
                if (i == 0) {
                    UnlockableDatas[i].Status = P_Unlockable_Status.Bought;
                }    
                if(i == 1) {
                    UnlockableDatas[i].Status = P_Unlockable_Status.Interactable;
                }
                UnlockableDatas[i].Cost = Mathf.RoundToInt((BaseCost * costMulti).RoundTo(CostRoundTo));
            }
            Save();
        }

        [BoxGroup("Main")]
        [Button]
        public virtual void SetupUnlockables(bool useMinMax = true) {
            if (useMinMax) SetupCurve();
            for (var i = 0; i < UnlockableDatas.Length; i++) {
                
                float costMulti = i.Remap(0f, (float)UnlockableDatas.Length - 1, 0f, 1f);
                costMulti = StationCostCurve.Evaluate(costMulti);


                float incomeMulti = i.Remap(0f, (float)UnlockableDatas.Length - 1, 0f, 1f);
                incomeMulti = StationCostCurve.Evaluate(incomeMulti);

                UnlockableDatas[i] = new P_Unlockable_Data();
                if (i == 0) {
                    UnlockableDatas[i].Status = P_Unlockable_Status.Bought;
                }    
                if(i == 1) {
                    UnlockableDatas[i].Status = P_Unlockable_Status.Interactable;
                }
                UnlockableDatas[i].Cost = Mathf.RoundToInt((BaseCost * costMulti).RoundTo(CostRoundTo));
            }
            
            Save();
        }

        protected virtual void SetupCurve(bool useMinMax = true) {
            StationCostCurve.MoveKey(0, new Keyframe(0, StationCostCurveRange.x));
            StationCostCurve.MoveKey(1, new Keyframe(1, StationCostCurveRange.y));
        }


        #region ScriptableSO

        [BoxGroup("Save")] [SerializeField] ScriptableObjectSaveInfo saveInfo;

        [ContextMenu("Save")]
        public void Save() {
            saveInfo.ModifyInfo(this, "Unlockable/LevelSettings", this.name);
            saveInfo.SaveScriptableObject();
        }

        [ContextMenu("Load")]
        public void Load() {
            saveInfo.LoadScriptableObject();
        }

        #endregion
    }
}