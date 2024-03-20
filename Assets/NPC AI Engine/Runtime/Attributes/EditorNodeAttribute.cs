using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Aikom.AIEngine
{
    [AttributeUsage(AttributeTargets.Class)]
    public class EditorNodeAttribute : Attribute
    {
        public string Description { get; private set; }

        public EditorNodeAttribute(string defaultDescription = "")
        {
            Description = defaultDescription;
        }
    }

}
