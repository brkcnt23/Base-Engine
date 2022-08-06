using System.Linq;
using Sirenix.Utilities;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Base;
using Sirenix.OdinInspector;
using UnityEngine;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;
using Sirenix.Utilities;
using UnityEditor.SceneManagement;
#endif

public class B_LevelCreator : SerializedMonoBehaviour {

    #if UNITY_EDITOR

    [SerializeField] private bool CanUse = true;
    
    [VerticalGroup("Level Creator")]
    [ValueDropdown("GetLevels")]
    [HideLabel]
    [DisableIf("@CanUse == false")]
    [OnValueChanged("LoadLevel")]
    public string SelectedLevel;

    [HideInInspector]
    public string lastOpenedLevel;
    private GameObject currentLevel;



    List<string> GetLevels() {
        var Levels = new List<string>();
        Levels.Add("Null");
        var levels = Resources.LoadAll(B_Database_String.Path_Res_MainLevels);
        for (int i = 0; i < levels.Length; i++) {
            Levels.Add(levels[i].name);
        }
        return Levels;
    }

    void LoadLevel() {
        if(SelectedLevel.IsNullOrWhitespace()) return;
        if (SelectedLevel == "Null") {
            SaveChanges();
            Clear(false);
            return;
        }
        if (currentLevel) {
            SaveChanges();
            Clear(false);
        }
        GameObject obj = Resources.Load<GameObject>(B_Database_String.Path_Res_MainLevels + SelectedLevel);
        currentLevel = PrefabUtility.InstantiatePrefab(obj, transform) as GameObject;
        PrefabUtility.UnpackPrefabInstance(currentLevel, PrefabUnpackMode.OutermostRoot, InteractionMode.AutomatedAction);
        AssetDatabase.SaveAssets();
    }
    [VerticalGroup("Level Creator")]
    [DisableIf("@CanUse == false")]
    [Button]
    void CreateNewLevel() {
        
        if (SelectedLevel.IsNullOrWhitespace() || SelectedLevel == "Null") {
            var allLevels = Resources.LoadAll<GameObject>(B_Database_String.Path_Res_MainLevels);
            var orderedEnumerable = allLevels.OrderBy(t => t.name);
            SelectedLevel = orderedEnumerable.Last().name;
        }
        
        if (currentLevel) {
            SaveChanges();
            Clear(false);
        }
        
        GameObject obj = Resources.Load<GameObject>(B_Database_String.Path_Res_MainLevels + SelectedLevel);
        int CurrentLevelsCount = Resources.LoadAll<GameObject>(B_Database_String.Path_Res_MainLevels).Length;
        string newLevelName = (CurrentLevelsCount + 1).ToString();
        if (newLevelName.Length < 3) {
            newLevelName = newLevelName.Insert(0, "0");
            if (newLevelName.Length < 3) {
                newLevelName = newLevelName.Insert(0, "0");
            }
        }
        newLevelName = newLevelName.Insert(0, "MainLevel ");
        currentLevel = Instantiate(obj, transform);
        currentLevel.name = newLevelName;
        PrefabUtility.SaveAsPrefabAsset(currentLevel, $"Assets/Resources/Levels/Mainlevels/{currentLevel.name}.prefab");
        GameObject newObj = Resources.Load<GameObject>(B_Database_String.Path_Res_MainLevels + newLevelName);
        DestroyImmediate(currentLevel);
        currentLevel = PrefabUtility.InstantiatePrefab(newObj, transform) as GameObject;
        PrefabUtility.UnpackPrefabInstance(currentLevel, PrefabUnpackMode.OutermostRoot, InteractionMode.AutomatedAction);
        SelectedLevel = newLevelName;
        AssetDatabase.SaveAssets();
    }
    
    
    [VerticalGroup("Level Creator", .5f)]
    [DisableIf("@CanUse == false")]
    [Button]
    void SaveChanges() {
        if(!currentLevel) return;
        PrefabUtility.SaveAsPrefabAsset(currentLevel, $"Assets/Resources/Levels/Mainlevels/{currentLevel.name}.prefab");
        AssetDatabase.SaveAssets();
    }
    [VerticalGroup("Level Creator", .5f)]
    [DisableIf("@CanUse == false")]
    [Button]
    void ResetChanges() {
        if(!currentLevel) return;
        transform.DestroyAllChildren();
        LoadLevel();
        AssetDatabase.SaveAssets();
    }
    
    [VerticalGroup("Level Creator", .5f)]
    [DisableIf("@CanUse == false")]
    [Button]
    public void Clear(bool clearLastLevel = true) {
        SaveChanges();
        transform.DestroyAllChildren();
        AssetDatabase.SaveAssets();
        lastOpenedLevel = SelectedLevel;
        if(clearLastLevel)
            SelectedLevel = "Null";
        currentLevel = null;
    }

    public void EditorPlaytimeClear() {
        SaveChanges();
        transform.DestroyAllChildren();
        AssetDatabase.SaveAssets();
        lastOpenedLevel = SelectedLevel;
        SelectedLevel = null;
        currentLevel = null;
        CanUse = false;
    }

    public async void EditorPlaytimeLoad(string levelName) {
        StartCoroutine(IE_EditorPlaytimeLoad(levelName));
    }

    private IEnumerator IE_EditorPlaytimeLoad(string levelName) {
        yield return new WaitForSeconds(.1f);
        Clear();
        this.SelectedLevel = levelName;
        LoadLevel();
        CanUse = true;
    }
    
    [VerticalGroup("Level Creator", .5f)]
    [Button]
    public void DeleteCurrentLevel() {

        if(GetLevels().Count < 3) {
            Debug.LogError("Can't delete last level");
            return;
        }
        string path = $"Assets/Resources/Levels/Mainlevels/{SelectedLevel}.prefab";
        Clear();
        AssetDatabase.DeleteAsset(path);
        AssetDatabase.Refresh();
        
    }
    #endif
}
