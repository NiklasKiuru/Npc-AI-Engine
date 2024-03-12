using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Aikom.AIEngine
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ContextVariableAttribute : Attribute
    {
        public CacheSpace Space { get; private set; }

        public ContextVariableAttribute(CacheSpace space)
        {
            Space = space;
        }
    }

}
