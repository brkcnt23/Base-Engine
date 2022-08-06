using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
using UnityEditor;
#endif
namespace Base.UI {
    [Serializable]
    public class UIManagerFunctions : B_ManagerBase {
        
        public override Task ManagerStrapping() {
            foreach (var item in Subframes) item.SetupFrame(this);
            B_GUIManager.SetupStaticFrame();
            B_GUIManager.ActivateAllPanels();

            return base.ManagerStrapping();
        }

        #region Helper Functions

        #region Getters

        private void AddChilds(Transform item) {
            foreach (Transform child in item) {
                if (child.GetComponent<MenuUISubframe>()) Subframes.Add(child.GetComponent<MenuUISubframe>());
                if (child.childCount > 0) AddChilds(child);
            }
        }

        #endregion

        #endregion
        
        #region Properties
        
        
        [ShowIf("OnEditor")]
        [BoxGroup("Editor Functions", false)]
        [PropertyTooltip("DO NOT ENABLE THIS IF YOU DON'T KNOW WHAT YOU ARE DOING")]
        public bool Admin;
        [ShowIf("AreYouSure")]
        [BoxGroup("Editor Functions")]
        [SerializeField] public List<MenuUISubframe> Subframes;
        
        [BoxGroup("Editor Functions", false)]
        [EnableIf("@MenuHolder == null")]
        [SerializeField] private Transform MenuHolder;
        
        #endregion

        #region Editor

#if UNITY_EDITOR
        
        #region Functions

        [ShowIf("OnEditor")]
        [VerticalGroup("Editor Functions/Upper", .5f)]
        [Button("Setup Subframes")]
        public async void SetupSubframes() {
            Subframes = new List<MenuUISubframe>();
            //Needs a better logic system for deciding when to do what
            if (MenuHolder.childCount != 5) AddEmptyMenus();
            AddChilds(MenuHolder);

            foreach (var item in Subframes) await item.SetupFrame(this);
            await SetNamesForSubframes();
            await CreateEnums();
            EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
        }
        [ShowIf("AreYouSure")]
        [VerticalGroup("Editor Functions/Upper", .5f)]
        [Button("Reset Subframes")]
        public async void ResetSubframes() {
            foreach (var subframe in Subframes) {
                foreach (var subcomponents in subframe.SubComponents) await subcomponents.FlushData();
                await subframe.FlushFrameData();
            }
            Subframes = new List<MenuUISubframe>();
        }

        [ShowIf("AreYouSure")]
        [VerticalGroup("Editor Functions/Upper", .5f)]
        [Button("Clear Subframes")]
        public void ClearSubframes() {
            for (var i = MenuHolder.childCount; i > 0; --i)
                GameObject.DestroyImmediate(MenuHolder.GetChild(0).gameObject);
            Subframes = new List<MenuUISubframe>();
        }

        [ShowIf("AreYouSure")]
        [HorizontalGroup("Editor Functions/Split", -.5f)]
        [Button("Set Names On Subframes", ButtonSizes.Small)]
        private Task SetNamesForSubframes() {
            for (var i = 0; i < Subframes.Count; i++) Subframes[i].name = Subframes[i].MenuType.ToString();
            return Task.CompletedTask;
        }

        [ShowIf("AreYouSure")]
        [HorizontalGroup("Editor Functions/Split", .5f)]
        [Button("Create SubModule Enums", ButtonSizes.Small)]
        private Task CreateEnums() {
            var TotalDuplicateCount = 0;
            var TotalComponentCount = 0;
            for (var i = 0; i < Subframes.Count; i++) {
                var enumGenericName = Subframes[i].MenuType + "Component";
                var names = new List<string>();
                for (var k = 0; k < Subframes[i].SubComponents.Count; k++) {
                    var _tempName = Subframes[i].SubComponents[k].ComponentParticularName;
                    var duplicates = Subframes[i].SubComponents.Where(t => t.ComponentParticularName == _tempName).ToArray();

                    if (duplicates.Count() <= 0) continue;
                    if (duplicates.Count() == 1) {
                        names.Add(duplicates[0].ComponentParticularName);
                        duplicates[0].EnumName = duplicates[0].ComponentParticularName;
                        TotalComponentCount++;
                    }
                    else {
                        for (var j = 0; j < duplicates.Count(); j++) {
                            if (names.Contains(duplicates[j].ComponentParticularName + "_" + j)) continue;
                            names.Add(duplicates[j].ComponentParticularName + "_" + j);
                            duplicates[j].EnumName = duplicates[j].ComponentParticularName + "_" + j;
                            if (j != 0) TotalDuplicateCount++;
                            TotalComponentCount++;
                        }
                    }
                }
                    if (names.Count <= 0) Debug.LogWarning("No Components Found, Please Add Components");
                else B_EnumCreator.CreateEnum(enumGenericName, names.ToArray());
            }
            Debug.Log(TotalComponentCount + " Components Found! " + TotalDuplicateCount + " Duplicates Renamed!");
            Debug.Log("Please SaveSystem The Unity Editor And Then Check If Enums Are Set");
            return Task.CompletedTask;
        }

        private bool Separated;
        private string SeparateButtonName = "Separate Menus";
        [VerticalGroup("Editor Functions/Upper", .5f)]
        [Button("$SeparateButtonName")]
        public void MenuAdjustment() {
            if (Separated) {
                for (var i = 1; i < Subframes.Count + 1; i++) Subframes[i - 1].MoveUIRect(new Vector3(0, 0, 0));
                SeparateButtonName = "Separate Menus";
            }
            else {
                for (var i = 1; i < Subframes.Count + 1; i++) Subframes[i - 1].MoveUIRect(new Vector3(1200 * i, 0, 0));
                SeparateButtonName = "Pull Menus";
            }
            Separated = !Separated;
        }

        #endregion

        #region Inspector Helpers

        private bool OnEditor() {
            return !EditorApplication.isPlaying;
        }
        private bool AreYouSure() {
            if (!EditorApplication.isPlaying && Admin) return true;
            return false;
        }
        private void AddEmptyMenus() {
            for (var i = 0; i < Enum.GetValues(typeof(Enum_MenuTypes)).Length; i++) {
                var obj = new GameObject();
                obj.AddComponent<RectTransform>();
                obj.transform.parent = MenuHolder;
                obj.transform.localPosition = Vector3.zero;
                obj.AddComponent(MenuType((Enum_MenuTypes)i).GetType());
                obj.GetComponent<MenuUISubframe>().MenuType = (Enum_MenuTypes)i;
                obj.GetComponent(MenuType(Enum_MenuTypes.Menu_GameOver).GetType());
            }
        }

        private MenuUISubframe MenuType(Enum_MenuTypes types) {
            switch (types) {
                case Enum_MenuTypes.Menu_Main:
                    return new Main();
                case Enum_MenuTypes.Menu_PlayerOverlay:
                    return new PlayerOverlay();
                case Enum_MenuTypes.Menu_Paused:
                    return new Paused();
                case Enum_MenuTypes.Menu_GameOver:
                    return new Gameover();
                case Enum_MenuTypes.Menu_Loading:
                    return new Loading();
                default:
                    return new Default();
            }
        }

        #endregion
#endif

        #endregion
    }
}