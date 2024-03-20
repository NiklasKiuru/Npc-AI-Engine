using System;

namespace Aikom.AIEngine
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ExposedVariableAttribute : Attribute
    {   
        public string Name { get; private set; }
        public string ToolTip { get; private set; }

        public ExposedVariableAttribute(string name, string toolTip = "") 
        { 
            Name = name;
            ToolTip = toolTip;
        }
    }
}

