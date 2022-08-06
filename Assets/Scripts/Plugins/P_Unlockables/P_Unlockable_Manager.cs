using System;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Base.Plugins.Unlockable {
    public class P_Unlockable_Manager : Singleton<P_Unlockable_Manager> {
        [BoxGroup("Settings")] public P_Unlockable_SO LevelSettings;
        [BoxGroup("Settings")] public P_Unlockable_Object[] Unlockables;
        [BoxGroup("Settings")] public Material GhostMaterial;

        [BoxGroup("Settings")] [SerializeField]
        protected bool PullFromParent;

        [BoxGroup("Settings")] [SerializeField]
        protected Transform UnlockableParent;

        private void Start() {
            Setup();
        }

        public virtual void Setup() {
            for (int i = 0; i < Unlockables.Length; i++) {
                Unlockables[i].Setup(LevelSettings.UnlockableDatas[i]);
            }

            Activate();
        }

        public virtual void Activate() {
        }

        public virtual void OnUnlockableBought(P_Unlockable_Object pUnlockableObject) {
        }


        #region Station Helpers

        public virtual P_Unlockable_Object GetUnlockable(int i) => Unlockables[i];

        public virtual P_Unlockable_Object EnableUnlockable(int i) =>
            Unlockables[i] ? Unlockables[i].MakeInteractable() : null;

        public virtual P_Unlockable_Object EnableNextUnlockable() =>
            Unlockables.FirstOrDefault(t => t.Data.Status == P_Unlockable_Status.Uninteractable)?.MakeInteractable();

        public virtual void EnableNextUnlockableN() =>
            Unlockables.FirstOrDefault(t => t.Data.Status == P_Unlockable_Status.Uninteractable)?.MakeInteractable();

        public virtual int UnlockableCount(P_Unlockable_Status status) =>
            Unlockables.Where(station => station.Data.Status == status).ToArray().Length;

        #endregion

        #region Editor

        [BoxGroup("Debug")] [OnValueChanged("ChangeShowcaseStatus")]
        public P_Unlockable_Status ShowcaseStatus;

        protected virtual void ChangeShowcaseStatus() {
            GetUnlockables();
            LevelSettings.SetupUnlockables(Unlockables);
            for (int i = 0; i < Unlockables.Length; i++) {
                Unlockables[i].CreateGhostObject(GhostMaterial);
                Unlockables[i].EditorVisualControlSetup(ShowcaseStatus);
            }
        }

        [BoxGroup("Debug")]
        [HorizontalGroup("Debug/Split", 0.5f)]
        [VerticalGroup("Debug/Split/right")]
        [Button("Auto Setup")]
        protected virtual void SetupLevelSettings() {
            GetUnlockables();
            LevelSettings.SetupUnlockables(Unlockables);
            for (int i = 0; i < Unlockables.Length; i++) {
                Unlockables[i].CreateGhostObject(GhostMaterial);
            }
        }

        [BoxGroup("Debug")]
        [VerticalGroup("Debug/Split/left")]
        [Button("Change Visuals Mode")]
        protected virtual void ChangeVisualsMode() {
            GetUnlockables();
            LevelSettings.SetupUnlockables(Unlockables);
            for (int i = 0; i < Unlockables.Length; i++) {
                Unlockables[i].ChangeVisualsMode();
            }
        }

        [ContextMenu("Create Ghost Object")]
        protected virtual void CreateGhostObjects() {
            GetUnlockables();
            LevelSettings.SetupUnlockables(Unlockables);
            for (int i = 0; i < Unlockables.Length; i++) {
                Unlockables[i].CreateGhostObject(GhostMaterial);
            }
        }

        [ContextMenu("Clear Ghost Object Data")]
        protected virtual void ClearGhostObjectData() {
            GetUnlockables();
            LevelSettings.SetupUnlockables(Unlockables);
            for (int i = 0; i < Unlockables.Length; i++) {
                Unlockables[i].ClearGhostObjectData();
            }
        }

        protected virtual void GetUnlockables() {
            if (PullFromParent) {
                Unlockables = UnlockableParent.GetComponentsInChildren<P_Unlockable_Object>();
            }

            if (Unlockables.IsNullOrEmpty()) {
                Debug.LogError("No Unlockables Found");
                return;
            }

        }

        #endregion

        #region Unlockables

        #endregion

        #region Singleton

        protected override void Awake() {
            base.Awake();
        }

        protected override void DestroyObject() {
            base.DestroyObject();
        }

        protected override void NullOut() {
            base.NullOut();
        }

        protected override void OnApplicationPause(bool pauseStatus) {
            base.OnApplicationPause(pauseStatus);
        }

        protected override void OnApplicationQuit() {
            base.OnApplicationQuit();
        }

        #endregion
    }
}