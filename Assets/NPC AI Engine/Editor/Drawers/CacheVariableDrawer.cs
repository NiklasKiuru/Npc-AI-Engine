using System.Reflection;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace Aikom.AIEngine.Editor
{   
    /// <summary>
    /// Drawer for cache variables
    /// </summary>
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

                bool disableSpaceEditing = false;
                var space = property.FindPropertyRelative("Space");
                var name = property.FindPropertyRelative("Name");
                CacheSpace lockedSpace = (CacheSpace)space.enumValueIndex;

                // This is a little bit hacky but its somewhat difficult to find an element from an array when you cant 
                // get the object value from it and for the attribute we need the node instance
                var listField = treeAsset.GetType().GetField("_nodes", BindingFlags.Instance | BindingFlags.NonPublic);
                if(int.TryParse(property.propertyPath.Split('[', ']')[1], out var nodeIndex))
                {
                    var list = (List<NodeBase>)listField.GetValue(treeAsset);
                    var node = list[nodeIndex];
                    var fields = node.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                    CacheVariableAttribute attr = null;
                    
                    foreach( var field in fields )
                    {
                        attr = field.GetCustomAttribute<CacheVariableAttribute>(false);
                        if (attr != null && field.Name.Equals(property.name))
                            break;
                        else
                            attr = null;
                    }
                    if(attr != null )
                    {
                        if(attr.RestrictSelection)
                        {
                            disableSpaceEditing = true;
                            lockedSpace = attr.Space;
                        }
                    }
                    
                }

                using (new EditorGUI.DisabledScope(disableSpaceEditing))
                {
                    var enumVal = EditorGUILayout.EnumPopup(lockedSpace, GUILayout.Width(100));
                    space.enumValueIndex = (int)(CacheSpace)enumVal;
                }

                var index = treeAsset.LocalVariables.IndexOf(name.stringValue);
                var selection = EditorGUILayout.Popup(index, treeAsset.LocalVariables.ToArray());
                if(selection != -1)
                    name.stringValue = treeAsset.LocalVariables[selection];

                EditorGUILayout.EndHorizontal();
            }
        }
    }

}
