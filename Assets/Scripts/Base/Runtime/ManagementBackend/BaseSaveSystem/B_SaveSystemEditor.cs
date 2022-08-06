using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
#endif
namespace Base {
    public class B_SaveSystemEditor {

        private List<B_SaveObject> _saveObjects;
        private readonly Dictionary<string, B_SaveObject> _savesDic;

        public B_SaveSystemEditor() {
            _saveObjects = new List<B_SaveObject>();
            _saveObjects = Resources.LoadAll<B_SaveObject>("SaveAssets").ToList();
            
            _savesDic = new Dictionary<string, B_SaveObject>();

            for (var i = 0; i < _saveObjects.Count; i++) {
                _saveObjects[i].LoadThisData();
                _savesDic.Add(_saveObjects[i].SaveName, _saveObjects[i]);
            }
        }
        public Task SaveSystemStrapping() {
            _saveObjects = new List<B_SaveObject>();
            _saveObjects = Resources.LoadAll<B_SaveObject>("SaveAssets").ToList();
            return Task.CompletedTask;
        }

        public object GetData(object saveEnum) {
            return GetSaveObject(saveEnum).GetData(saveEnum);
        }

        public B_SaveObject GetSaveObject(object saveEnum) {
            string type = saveEnum.GetType().ToString().Replace("Enum_", "");
            if (!_savesDic.ContainsKey(type)) return null;
            return _savesDic[type];
        }

        public void SaveAllData() {
            foreach (var item in _saveObjects) item.SaveThisData();
        }


        #region Editor Functions

#if UNITY_EDITOR
        [BoxGroup("New SaveSystem Object", centerLabel: true, order: 0)]
        [InfoBox("$InfoBoxString", "IsntReadyForSave")]
        [InlineEditor(ObjectFieldMode = InlineEditorObjectFieldModes.Hidden)]
        public B_SaveObject newBSaveObject;

        public B_SaveSystemEditor(OdinMenuTree tree) {
            _saveObjects = new List<B_SaveObject>();
            _saveObjects = Resources.LoadAll<B_SaveObject>("SaveAssets").ToList();
            _savesDic = new Dictionary<string, B_SaveObject>();
            for (var i = 0; i < _saveObjects.Count; i++) _savesDic.Add(_saveObjects[i].SaveName, _saveObjects[i]);

            newBSaveObject = ScriptableObject.CreateInstance<B_SaveObject>();
        }

        [ShowIf("IsReadyForSave")]
        [VerticalGroup("Main Functions")]
        [GUIColor("getGreen")]
        [Button]
        public void CreateNewSave() {
            var obj = newBSaveObject;
            if (obj.SaveName.MakeViable() == "") obj.SaveName = "Empty_Save_Name";
            else obj.SaveName = obj.SaveName.MakeViable();
            obj.name = obj.SaveName;
            obj.SaveThisData();
            obj.Created = true;
            AssetDatabase.CreateAsset(obj, "Assets/Resources/SaveAssets/" + obj.SaveName + ".asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            _saveObjects.Add(obj);
            CreateEnums();
            newBSaveObject = ScriptableObject.CreateInstance<B_SaveObject>();
        }

        [VerticalGroup("Main Functions")]
        [GUIColor("getRed")]
        [Button]
        public void ClearAllSaves() {
            string[] assetsPath = { "Assets/Resources/SaveAssets" };
            foreach (var item in AssetDatabase.FindAssets("", assetsPath)) {

                var path = AssetDatabase.GUIDToAssetPath(item);
                var _saveObject = AssetDatabase.LoadAssetAtPath(path, typeof(B_SaveObject)) as B_SaveObject;



                if (!_saveObject.IsPermanent) {
                    _saveObjects.Remove(_saveObject);
                    Debug.Log(_saveObject.SaveEnumLocations);
                    File.Delete(_saveObject.SaveEnumLocations);
                    AssetDatabase.DeleteAsset(path);
                    _saveObject.DeleteThisData();
                }

            }
            CreateEnums();
            AssetDatabase.SaveAssets();
        }
        [HideIf("IsReadyForSave")]
        [VerticalGroup("Main Functions")]
        [GUIColor("getGray")]
        [Button]
        public void CreateEnums() {
            var _temp = new string[_saveObjects.Count];
            for (var i = 0; i < _saveObjects.Count; i++) {
                _temp[i] = _saveObjects[i].SaveName;
                _saveObjects[i].CreateEnums();
            }
            B_EnumCreator.CreateEnum("Saves", _temp);
        }

        private bool IsReadyForSave() {
            if (newBSaveObject == null) return false;
            if (string.IsNullOrEmpty(newBSaveObject.SaveName)) return false;
            if (newBSaveObject.SaveName.Length <= 3) return false;
            if (newBSaveObject.SaveCluster.Count < 1) return false;
            for (var i = 0; i < newBSaveObject.SaveCluster.Count; i++)
                if (newBSaveObject.SaveCluster[i].Name.IsVaibleForSave() != B_ExtentionFunctions.SaveNameViabilityStatus.Viable)
                    return false;
            return true;
        }

        private bool IsntReadyForSave() {
            return !IsReadyForSave();
        }

        public string InfoBoxString() {
            if (newBSaveObject == null) return null;
            if (string.IsNullOrEmpty(newBSaveObject.SaveName) || newBSaveObject.SaveName.Length <= 3) return "Enter A Name";
            if (newBSaveObject.SaveCluster.Count < 1) return "Enter Atleast One Data";
            for (var i = 0; i < newBSaveObject.SaveCluster.Count; i++)
                if (newBSaveObject.SaveCluster[i].Name.IsVaibleForSave() != B_ExtentionFunctions.SaveNameViabilityStatus.Viable)
                    return "Enter Atleast One Data";
            return null;
        }

        private Color GetButtonColor() {
            GUIHelper.RequestRepaint();
            return Color.HSVToRGB(Mathf.Cos((float)EditorApplication.timeSinceStartup + 1f) * 0.225f + 0.325f, 1, 1);
        }

        #region Inspector Window Functions

        private Color getGreen() {
            return Color.green;
        }
        private Color getRed() {
            return Color.red;
        }
        private Color getYellow() {
            return Color.yellow;
        }
        private Color getGray() {
            return Color.gray;
        }

        #endregion
#endif

        #endregion
    }
}