using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Base {
    public static class B_ExtentionFunctions {
        
        #region Transform Extentions

        public static void ResizeObject(this Transform objToEnlarge, float Size) {
            objToEnlarge.localScale = new Vector3(Size, Size, Size);
        }

        //Rework
        public static IEnumerable GetAllChilrenOnTransform(this Transform transform) {
            List<Transform> transforms = new List<Transform>();
            foreach (Transform item in transform) {
                transforms.Add(item);
            }

            return transforms;
        }

        public static void DestroyAllChildren(this Transform transform) {
            if (transform.childCount <= 0) return;
            for (int i = transform.childCount - 1; i >= 0; i--) {
#if UNITY_EDITOR
                if (!Application.isPlaying)
                    GameObject.DestroyImmediate(transform.GetChild(i).gameObject);
#endif
                if (Application.isPlaying)
                    GameObject.Destroy(transform.GetChild(i).gameObject);
            }
        }

        
        #endregion Transform Extentions

        #region Recttransform Extentions

        //Use this to move Pesky uı objects
        public static void MoveUIObject(this RectTransform rectTransform, Vector2 vector2) {
            rectTransform.offsetMax = vector2;
            rectTransform.offsetMin = vector2;
        }

        #endregion Recttransform Extentions

        #region Vector3 Extentions

        public static Vector3 GetWorldPosition(Ray ray, LayerMask Mask) {
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, Mask)) return hit.point;
            return Vector3.zero;
        }

        public static Vector3 GetWorldPosition(this Vector3 vector3, Camera cam, LayerMask Mask) {
            var ray = cam.ScreenPointToRay(vector3);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, Mask)) return hit.point;
            return Vector3.zero;
        }

        public static Transform GetWorldObject(this Vector3 vec3, Camera cam, LayerMask mask) {
            var ray = cam.ScreenPointToRay(vec3);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask)) return hit.collider.transform;
            return null;
        }

        public static Vector3 GetHitPosition(this Vector3 mainObj, Vector3 objectToPush, float yMinus, float force) {
            var _temp = mainObj;
            _temp.y -= yMinus;
            return (objectToPush - _temp) * force;
        }

        public static Vector3 FindCenterInGroup<T>(this IEnumerable<T> ObjectGroup) where T : MonoBehaviour {
            MonoBehaviour[] objects = ObjectGroup.ToArray();
            if (objects.Length == 0)
                return Vector3.zero;
            if (objects.Length == 1)
                return objects[0].transform.position;
            var bounds = new Bounds(objects[0].transform.position, Vector3.zero);
            for (var i = 1; i < objects.Length; i++) {
                if (objects[i] != null)
                    bounds.Encapsulate(objects[i].transform.position);
            }

            return bounds.center;
        }

        #endregion Vector3 Extentions

        #region Math Extentions

        public static float Round(float value, int digits) {
            var mult = Mathf.Pow(10.0f, digits);
            return Mathf.Round(value * mult) / mult;
        }

        //Round a given number to nearest 100
        public static float RoundTo(this float value) {
            return Mathf.Round(value / 100) * 100;
        }

        public static float RoundTo(this float value, float to = 1) {
            return Mathf.Round(value / to) * to;
        }

        public static float RoundTo(this int value) {
            return Mathf.Round(value / 100) * 100;
        }

        public static float RoundTo(this int value, float to = 1) {
            return Mathf.Round(value / to) * to;
        }

        public static float Multi(this float value, float multiplier) {
            return value * multiplier;
        }


        public static float Remap(this float value, float from1, float to1, float from2, float to2, bool clamp = true) {
            float x = (value - from1) / (to1 - from1) * (to2 - from2) + from2;
            if (from2 < to2)
                return !clamp ? x : Mathf.Clamp(x, from2, to2);
            else
                return !clamp ? x : Mathf.Clamp(x, to2, from2);
        }

        public static float Remap(this int value, float from1, float to1, float from2, float to2, bool clamp = true) {
            float x = (value - from1) / (to1 - from1) * (to2 - from2) + from2;

            if (from2 < to2)
                return !clamp ? x : Mathf.Clamp(x, from2, to2);
            else
                return !clamp ? x : Mathf.Clamp(x, to2, from2);
        }


        public static float ClampAngle(float angle, float min, float max) {
            angle = Mathf.Repeat(angle, 360);
            min = Mathf.Repeat(min, 360);
            max = Mathf.Repeat(max, 360);
            var inverse = false;
            var tmin = min;
            var tangle = angle;
            if (min > 180) {
                inverse = !inverse;
                tmin -= 180;
            }

            if (angle > 180) {
                inverse = !inverse;
                tangle -= 180;
            }

            var result = !inverse ? tangle > tmin : tangle < tmin;
            if (!result)
                angle = min;

            inverse = false;
            tangle = angle;
            var tmax = max;
            if (angle > 180) {
                inverse = !inverse;
                tangle -= 180;
            }

            if (max > 180) {
                inverse = !inverse;
                tmax -= 180;
            }

            result = !inverse ? tangle < tmax : tangle > tmax;
            if (!result)
                angle = max;
            return angle;
        }

        #endregion Math Extentions

        #region Gameobject Extentions

        public static T InstantiateB<T>(this T obj) where T : MonoBehaviour {
            T returnobj = GameObject.Instantiate(obj, B_LevelControl.CurrentLevelObject);
            return returnobj;
        }

        public static T InstantiateB<T>(this T obj, Vector3 position) where T : MonoBehaviour {
            T returnobj = GameObject.Instantiate(obj, B_LevelControl.CurrentLevelObject);
            returnobj.transform.position = position;
            return returnobj;
        }

        public static T InstantiateB<T>(this T obj, Vector3 position, Quaternion rotation) where T : MonoBehaviour {
            T returnobj = GameObject.Instantiate(obj, B_LevelControl.CurrentLevelObject);
            returnobj.transform.position = position;
            returnobj.transform.rotation = rotation;
            return returnobj;
        }
        
        public static bool CheckLayer(this GameObject go, LayerMask layerMask)
        {
            return layerMask == (layerMask | (1 << go.layer));
        }

        /// <summary>
        /// Takes in a GameObject and copies it, needs a material to ghost it
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="ghostMaterial"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static GameObject CreateGhostObject(this GameObject obj, Material ghostMaterial, Transform parent = null) {
            if (parent == null) {
                parent = B_LevelControl.CurrentLevelObject;
            }
            
            GameObject ghostObject = GameObject.Instantiate(obj, parent);

            List<GameObject> meshes = new List<GameObject>();
            foreach (var x in ghostObject.GetComponentsInChildren<MeshRenderer>()) {
                meshes.Add(x.gameObject);
            }

            for (int i = 0; i < meshes.Count; i++) {
                meshes[i].TryGetComponent(out Renderer rends);
                Material[] materials = new Material[rends.sharedMaterials.Length];
                for (int x = 0; x < rends.sharedMaterials.Length; x++) {
                    materials[x] = ghostMaterial;
                }
                rends.sharedMaterials = materials;
            }

            return ghostObject;
        }

        #endregion

        #region String Extentions

        public static bool IsAllLetters(this string s) {
            foreach (var c in s)
                if (!char.IsLetter(c))
                    return false;
            return true;
        }

        public static bool IsAllDigits(this string s) {
            foreach (var c in s)
                if (!char.IsDigit(c))
                    return false;
            return true;
        }

        public static bool IsAllLettersOrDigits(this string s) {
            foreach (var c in s)
                if (!char.IsLetterOrDigit(c))
                    return false;
            return true;
        }

        public static float IsFloat(this string s) {
            if (s.IsAllDigits())
                return float.Parse(s);
            return 0;
        }

        public enum SaveNameViabilityStatus {
            Viable,
            Null,
            Incomplete,
            HasDigits
        }

        public static SaveNameViabilityStatus IsVaibleForSave(this string obj) {
            if (obj.Length <= 3 && !(obj == null || obj == "Null" || string.IsNullOrEmpty(obj)))
                return SaveNameViabilityStatus.Incomplete;
            if (obj == null || obj == "Null" || string.IsNullOrEmpty(obj)) return SaveNameViabilityStatus.Null;
            if (obj.Any(char.IsDigit)) return SaveNameViabilityStatus.HasDigits;
            // if(obj.Any(char.spa))
            return SaveNameViabilityStatus.Viable;
        }

        public static string MakeViable(this string obj) {
            switch (obj.IsVaibleForSave()) {
                case SaveNameViabilityStatus.Viable:
                    return obj;
                case SaveNameViabilityStatus.Null:
                    Debug.Log("Name was " + obj.IsVaibleForSave());
                    return "";
                case SaveNameViabilityStatus.Incomplete:
                    Debug.Log(obj + " Was " + obj.IsVaibleForSave());
                    return obj + "_Completed_Part";
                case SaveNameViabilityStatus.HasDigits:
                    Debug.Log(obj + " " + obj.IsVaibleForSave());
                    var newObj = obj.Where(t => !char.IsDigit(t)).ToArray();
                    var newName = new string(newObj);
                    if (newName.Where(t => char.IsLetter(t)).ToArray().Length <= 3) newName = "";
                    return newName;
            }

            return null;
        }
        

        #endregion String Extentions

        #region AssetDatabase Extentions

        /// <summary>
        /// Only works on editor
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<T> FindAssetsByType<T>() where T : UnityEngine.Object {
#if UNITY_EDITOR
            List<T> assets = new List<T>();
            string[] guids = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(T)));
            for (int i = 0; i < guids.Length; i++) {
                string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
                if (asset != null) {
                    assets.Add(asset);
                }
            }

            return assets;
#else
            return null;
#endif
        }

        public static List<string> FindAssetsPathByType<T>() where T : UnityEngine.Object {
#if UNITY_EDITOR
            List<string> assets = new List<string>();
            string[] guids = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(T)));
            for (int i = 0; i < guids.Length; i++) {
                string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
                if (asset != null) {
                    assets.Add(assetPath);
                }
            }

            return assets;
#else
            return null;
#endif
        }

        public static string FindAssetParenthPath(this string originalPath) {
#if UNITY_EDITOR
            return Directory.GetParent(originalPath).ToString().Replace("\\", "/");
#else
            return null;
#endif
        }

        public static string FindAssetPath<T>() where T : UnityEngine.Object {
#if UNITY_EDITOR

            string assetPath = "";
            string[] guids = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(T)));
            for (int i = 0; i < guids.Length; i++) {
                assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
                if (asset != null) {
                    assetPath = assetPath.Replace("\\", "/");
                }
            }

            return assetPath;

#else
            return null;
#endif
        }

        #endregion

        #region Scriptable Object Extentions

        #region Save Extentions

        public static void SaveScriptableObject(this ScriptableObjectSaveInfo obj) {
            string path = $"{Application.persistentDataPath}/{obj.foldername}";
            if (!Directory.Exists(path)) {
                Directory.CreateDirectory(path);
            }

            path += $"/{obj.filename}.scs";
            string saveData = JsonUtility.ToJson(obj.obj, true);
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(path);
            bf.Serialize(file, saveData);
            file.Close();
        }

        public static void LoadScriptableObject(this ScriptableObjectSaveInfo obj) {
            if (obj.SaveExists()) {
                BinaryFormatter bf = new BinaryFormatter();
                string path = $"{Application.persistentDataPath}/{obj.foldername}/{obj.filename}.scs";
                FileStream file = File.Open(path, FileMode.Open);
                JsonUtility.FromJsonOverwrite(bf.Deserialize(file).ToString(), obj.obj);
                file.Close();
            }
        }

        public static bool SaveExists(this ScriptableObjectSaveInfo obj) {
            return File.Exists($"{Application.persistentDataPath}/{obj.foldername}/{obj.filename}.scs");
        }

        public static void ClearScriptableObject(this ScriptableObjectSaveInfo obj) {
            if (obj.SaveExists()) {
                string path = $"{Application.persistentDataPath}/{obj.foldername}/{obj.filename}.scs";
                File.Delete(path);
            }
        }

        #endregion

        #endregion

        #region Coroutine Extentions

        /// <summary>
        /// Simply Runs the enumarator without any return
        /// </summary>
        /// <param name="enumerator"></param>
        public static Coroutine RunCoroutine(this IEnumerator enumerator) {
            return B_CoroutineControl.Queue.RunCoroutine(enumerator);
        }

        /// <summary>
        /// Runs the enumarator with delay without any return
        /// </summary>
        /// <param name="enumerator"></param>
        /// <param name="delay"></param>
        public static Coroutine RunCoroutine(this IEnumerator enumerator, float delay) {
            return B_CoroutineControl.Queue.RunCoroutine(enumerator, delay);
        }

        /// <summary>
        /// Runs the enumarator
        /// </summary>
        /// <param name="enumerator"></param>
        /// <param name="coroutine"></param>
        public static void RunCoroutine(this IEnumerator enumerator, Coroutine coroutine) {
            B_CoroutineControl.Queue.RunCoroutine(enumerator, coroutine);
        }

        /// <summary>
        /// Runs the enumarator with delay
        /// </summary>
        /// <param name="enumerator"></param>
        /// <param name="coroutine"></param>
        /// <param name="delay"></param>
        public static void RunCoroutine(this IEnumerator enumerator, Coroutine coroutine, float delay) {
            B_CoroutineControl.Queue.RunCoroutine(enumerator, coroutine, delay);
        }

        /// <summary>
        /// Runs the function in a coroutine
        /// </summary>
        /// <param name="enumerator"></param>
        /// <param name="delay"></param>
        public static Coroutine RunWithDelay(Action method, float delay) {
            return B_CoroutineControl.Queue.RunFunctionWithDelay(method, delay);
        }

        public static Coroutine StopCoroutine(this Coroutine coroutine) {
            return B_CoroutineControl.Queue.StopCoroutine(coroutine);
        }

        #endregion

        #region IList Extentions

        /// <summary>
        /// Get random element from list or array. Returns null if list is empty.
        /// </summary>
        /// <param name="list"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetRandom<T>(this IList<T> list) {
            return list[UnityEngine.Random.Range(0, list.Count)];
        }
        
        /// <summary>
        /// Returns a shuffled copy of the list.
        /// </summary>
        /// <param name="list"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<T> Shuffle<T>(this IList<T> list) {
            List<T> shuffled = new List<T>(list);
            for (int i = 0; i < shuffled.Count; i++) {
                int randomIndex = UnityEngine.Random.Range(0, shuffled.Count);
                (shuffled[i], shuffled[randomIndex]) = (shuffled[randomIndex], shuffled[i]);
            }
            return shuffled;
        }
        
        /// <summary>
        /// Returns a value between the desired amount and the max amount.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="desired"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static int GetMaxInList<T>(this IList<T> list, int desired = -1) {
            if (desired == -1)  return list.Count;
            return desired > list.Count ? list.Count : desired;
        }
        
        public static bool IsNullOrEmpty<T>(this IList<T> list) => list == null || list.Count == 0;
        
        #endregion
        
        #region Collider Extentions

        public static Vector3 GetRandomPoint(this Collider collider) {
            return new Vector3(
                Random.Range(collider.bounds.min.x, collider.bounds.max.x),
                Random.Range(collider.bounds.min.y, collider.bounds.max.y),
                Random.Range(collider.bounds.min.z, collider.bounds.max.z)
            );
        }

        public static Vector3 GetRandomPoint(this Collider collider, float extends) {
            return new Vector3(
                Random.Range(collider.bounds.min.x, collider.bounds.max.x) * extends,
                Random.Range(collider.bounds.min.y, collider.bounds.max.y) * extends,
                Random.Range(collider.bounds.min.z, collider.bounds.max.z) * extends
            );
        }

        #endregion

        #region Color Extentions
        
        public static Color SetAlpha(this Color color, float alpha)
        {
            color.a = alpha;
            return color;
        }

        #endregion

        #region Animator

        public static string[] GetClipNames(this Animator animator) {
            return animator.runtimeAnimatorController.animationClips.Select(clip => clip.name).ToArray();
        }
        
        public static string[] GetClipHash(this Animator animator) {
            string[] clipHashes = animator.runtimeAnimatorController.animationClips.Select(clip => Animator.StringToHash(clip.name).ToString()).ToArray();
            return clipHashes;
        }

        public static void PlayClip(this Animator animator, string name, float crossFade = 0f, int layer = 0) {
            animator.CrossFade(name, crossFade, layer);
        }
        
        public static void PlayClip(this Animator animator, int name, float crossFade = 0f, int layer = 0) {
            animator.CrossFade(name, crossFade, layer);
        }
        
        public static void PlayClipFixedTime(this Animator animator, string name, float crossFade = 0f, int layer = 0) {
            animator.CrossFadeInFixedTime(name, crossFade, layer);
        }

#if UNITY_EDITOR
        public static void CreateAnimationEnums(this Animator animator, string suffix = "", string prefix = "") {
            B_EnumCreator.CreateEnum(animator.runtimeAnimatorController.name,
                animator.GetClipNames(), animator.GetClipHash(), suffix, prefix);
        }
        

        
        
#endif


        #endregion


    }
}

[Serializable]
public class ScriptableObjectSaveInfo {
    [HideInInspector] public ScriptableObject obj;
    public string foldername, filename;

    public ScriptableObjectSaveInfo(ScriptableObject obj, string foldername, string filename) {
        this.obj = obj;
        this.foldername = foldername;
        this.filename = filename;
    }

    public void ModifyInfo(ScriptableObject obj, [CanBeNull] string foldername, [CanBeNull] string filename) {
        if (obj) this.obj = obj;
        if (foldername.Length > 0) this.foldername = foldername;
        if (filename.Length > 0) this.filename = filename;
    }

    public ScriptableObjectSaveInfo(ScriptableObject scriptableObject) : this(scriptableObject, null, null) {
    }

    public ScriptableObjectSaveInfo(ScriptableObject scriptableObject, string filename) : this(scriptableObject, null,
        filename) {
    }
}