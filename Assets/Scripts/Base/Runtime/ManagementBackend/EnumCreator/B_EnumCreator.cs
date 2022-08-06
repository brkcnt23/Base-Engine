using System.IO;
using System.Linq;
using Unity.Tutorials.Core.Editor;
using UnityEditor;

namespace Base {
    public static class B_EnumCreator {
#if UNITY_EDITOR
        public static string BasePath = "Assets/Resources/EnumStorage/";

        // public static void CreateEnum(string ItemName, string[] ItemsToEnum) {
        //     var Item = ItemName + ".cs";
        //     var AllPath = BasePath + Item;
        //
        //     var FileInside = "public enum Enum_" + ItemName + "{";
        //     if (ItemsToEnum.Length > 0)
        //         foreach (var item in ItemsToEnum) {
        //             FileInside += " " + item;
        //             if (item != ItemsToEnum.Last())
        //                 FileInside += ",";
        //             else FileInside += "}";
        //         }
        //     else FileInside += "}";
        //     File.WriteAllText(AllPath, FileInside);
        //     AssetDatabase.Refresh();
        //
        // }

        public static void CreateEnum(string ItemName, string[] ItemsToEnum, string suffix = "", string prefix = "") {
            if (suffix.IsNullOrEmpty()) {
                suffix = "Enum_";
            }

            var Item = ItemName + prefix + ".cs";
            var AllPath = BasePath + Item;

            var FileInside = "public enum " + suffix + ItemName + "{";
            if (ItemsToEnum.Length > 0)
                foreach (var item in ItemsToEnum) {
                    FileInside += " " + item;
                    if (item != ItemsToEnum.Last())
                        FileInside += ",";
                    else FileInside += "}";
                }
            else FileInside += "}";

            File.WriteAllText(AllPath, FileInside);
            AssetDatabase.Refresh();
        }

        public static void CreateEnum(string ItemName, string[] ItemsToEnum, string[] itemValues, string suffix = "", string prefix = "") {
            if (suffix.IsNullOrEmpty()) {
                suffix = "Enum_";
            }

            var Item = ItemName + prefix + ".cs";
            var AllPath = BasePath + Item;

            var FileInside = "public enum " + suffix + ItemName + "{";
            if (ItemsToEnum.Length > 0)
                for (int i = 0; i < ItemsToEnum.Length; i++) {
                    FileInside += " " + ItemsToEnum[i] + " = " + itemValues[i];
                    if (ItemsToEnum[i] != ItemsToEnum.Last())
                        FileInside += ",";
                    else FileInside += "}";
                }
            else FileInside += "}";

            File.WriteAllText(AllPath, FileInside);
            AssetDatabase.Refresh();
        }
#endif
    }
}