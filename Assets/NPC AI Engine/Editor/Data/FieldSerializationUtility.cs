using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Reflection;
using System;
using Codice.CM.SEIDInfo;
using UnityEditor.UIElements;
using UnityEditor;
using System.Xml.Linq;

namespace Aikom.AIEngine.Editor
{
    public class FieldSerializationUtility
    {
        public static VisualElement[] GetPropertyElements(NodeBase node)
        {
            FieldInfo[] infoArray = node.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            var validFields = new List<ExposedVariableInfo>();
            foreach(FieldInfo field in infoArray)
            {   
                var exposedAttr = field.GetCustomAttribute<ExposedVariableAttribute>();
                if( exposedAttr != null )
                    validFields.Add(new ExposedVariableInfo 
                    { 
                        Attribute = exposedAttr,
                        Field = field,
                    });
            }
            var elements = new VisualElement[validFields.Count];
            for(int i = 0; i < elements.Length; i++)
            {
                var attr = validFields[i];
                elements[i] = GenerateElement(attr, node);
            }
            return elements;
        }

        private static VisualElement GenerateElement(ExposedVariableInfo info, NodeBase node)
        {
            VisualElement returnVal = null;
            Type fieldType = info.Field.FieldType;
            if(fieldType == typeof(string))
            {
                returnVal = new TextField(info.Attribute.Name) { value = (string)info.Field.GetValue(node) };
                returnVal.RegisterCallback<ChangeEvent<string>>((evt) => { info.Field.SetValue(node, evt.newValue); });
            }
            if (fieldType == typeof(int))
            {
                returnVal = new IntegerField(info.Attribute.Name) { value = (int)info.Field.GetValue(node) };
                returnVal.RegisterCallback<ChangeEvent<int>>((evt) => { info.Field.SetValue(node, evt.newValue); });
            }
            if (fieldType == typeof(float))
            {
                returnVal = new FloatField(info.Attribute.Name) { value = (float)info.Field.GetValue(node) };
                returnVal.RegisterCallback<ChangeEvent<float>>((evt) => { info.Field.SetValue(node, evt.newValue); });
            }
            if (fieldType == typeof(bool))
            {
                returnVal = new Toggle(info.Attribute.Name) { value = (bool)info.Field.GetValue(node) };
                returnVal.RegisterCallback<ChangeEvent<bool>>((evt) => { info.Field.SetValue(node, evt.newValue); });
            }
            if (fieldType == typeof(Type))
            {
                var list = new List<string>();
                foreach(var type in TypeCache.GetTypesDerivedFrom<MonoBehaviour>())
                {   
                    if(type.GetCustomAttribute<DiscoverableAttribute>() != null)
                        list.Add(type.ToString());
                }
                var dropDown = new DropdownField(info.Attribute.Name, list, 0);
                dropDown.RegisterValueChangedCallback((evt) => 
                {
                    var type = Type.GetType(evt.newValue);
                    if(type != null)
                        info.Field.SetValue(node, evt.newValue);
                });
                returnVal = dropDown;
            }
            if(fieldType.IsEnum)
            {   
                Enum val = (Enum)info.Field.GetValue(node);
                
                // Flagsfield
                if (val.GetType().GetCustomAttribute<FlagsAttribute>() != null)
                {
                    returnVal = new EnumFlagsField(info.Attribute.Name, val) { value = val };
                    returnVal.RegisterCallback<ChangeEvent<Enum>>((evt) => { info.Field.SetValue(node, evt.newValue); });
                }
                else
                {
                    returnVal = new EnumField(info.Attribute.Name, val) { value = val };
                    returnVal.RegisterCallback<ChangeEvent<Enum>>((evt) => { info.Field.SetValue(node, evt.newValue); });
                }
            }

            if(fieldType == typeof(LayerMask))
            {
                var mask = (LayerMask)info.Field.GetValue(node);
                returnVal = new LayerMaskField(info.Attribute.Name) { value = mask };
                returnVal.RegisterCallback<ChangeEvent<int>>((evt) => {
                    var val = (LayerMask)evt.newValue;
                    info.Field.SetValue(node, val); 
                });
            }

            if(fieldType == typeof(Vector3))
            {
                returnVal = new Vector3Field(info.Attribute.Name) { value = (Vector3)info.Field.GetValue(node) };
                returnVal.RegisterCallback<ChangeEvent<Vector3>>((evt) => { info.Field.SetValue(node, evt.newValue); });
            }

            if (fieldType == typeof(Vector2))
            {
                returnVal = new Vector2Field(info.Attribute.Name) { value = (Vector2)info.Field.GetValue(node) };
                returnVal.RegisterCallback<ChangeEvent<Vector2>>((evt) => { info.Field.SetValue(node, evt.newValue); });
            }

            if (returnVal == null)
                Debug.LogError("Used variable type not supported: " + fieldType.ToString());
            else if(info.Attribute.ToolTip != string.Empty)
                returnVal.tooltip = info.Attribute.ToolTip;

            return returnVal;
        }
    }

    public struct ExposedVariableInfo
    {
        public ExposedVariableAttribute Attribute;
        public FieldInfo Field;
    }
}
