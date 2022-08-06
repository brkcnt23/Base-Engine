using UnityEditor;
using UnityEngine;
namespace Base.Editor.Attributes.Test {
    [CustomPropertyDrawer(typeof(Test1Attribute))]
    public class Test1PropertyDrawer : PropertyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            GUI.enabled = false;
            EditorGUI.PropertyField(position, property, label);
            GUI.enabled = true;
        }
    }
}