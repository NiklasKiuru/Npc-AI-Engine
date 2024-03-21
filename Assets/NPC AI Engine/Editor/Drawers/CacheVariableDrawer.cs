using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace Aikom.AIEngine.Editor
{
    [CustomPropertyDrawer(typeof(CacheVariable))]
    public class CacheVariableDrawer : PropertyDrawer
    {   
        // Gets rid of empty space in the inspector
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) { return 0; }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {   
            // **TODO** : Replace cast to check if the object is INodeCreationContext
            var treeAsset = property.serializedObject.targetObject as TreeAsset;
            EditorGUILayout.LabelField(label);
            if(treeAsset != null)
            {   
                EditorGUILayout.BeginHorizontal();
                var space = property.FindPropertyRelative("Space");
                var name = property.FindPropertyRelative("Name");
                var enumVal = EditorGUILayout.EnumPopup((CacheSpace)space.enumValueIndex, GUILayout.Width(100));
                space.enumValueIndex = (int)(CacheSpace)enumVal;

                var index = treeAsset.LocalVariables.IndexOf(name.stringValue);
                var selection = EditorGUILayout.Popup(index, treeAsset.LocalVariables.ToArray());
                if(selection != -1)
                    name.stringValue = treeAsset.LocalVariables[selection];

                EditorGUILayout.EndHorizontal();
            }
        }
    }

}
